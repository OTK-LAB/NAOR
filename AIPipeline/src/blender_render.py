"""
Blender headless render stage of the AI sprite pipeline (Phase 6.2).

Takes a .bvh skeletal animation (produced upstream by the MoMask
text-to-motion stage, see text_to_motion.py) and renders it into two
per-frame control image sequences for ComfyUI's SDXL + ControlNet-Depth
stylization stage:

  frame_%04d.png  -- "beauty" render: flat matte-gray shaded body, side
                     view, transparent background.
  depth_%04d.png  -- ControlNet-Depth-convention depth map: bright (near)
                     figure on a pure black (far) background, normalized
                     per frame over the figure's own depth range so the
                     figure spans roughly 30-100% brightness.

The BVH skeleton alone has no volume, so this script builds a simple
capsule-like body mesh (one tapered cylinder per bone, skinned to the
armature via vertex groups + an Armature modifier) purely to give the
depth/beauty passes something to render. It is a stylized silhouette, not
an anatomical model. Note on Blender 5.x: the classic Map Range / File
Output compositor-node depth technique no longer works as documented
online (the compositor node tree moved to scene.compositing_node_group
and File Output nodes never fired headlessly in testing); this script
instead extracts depth via a per-material Camera-Data shader +
view_layer.material_override, which does work headlessly in Blender 5.1.

Invocation (must be run *inside* Blender, not as a plain python3 script):

    /Applications/Blender.app/Contents/MacOS/Blender -b \\
        -P AIPipeline/src/blender_render.py -- \\
        <bvh_path> <output_dir> [--frames N] [--res R] [--prop sword] \\
        [--lock-facing]

    N defaults to 16 (frames uniformly sampled across the BVH's animation
    range). R defaults to 1024 (SDXL-native square resolution). --prop sword
    (default: no prop) attaches a simple greatsword mesh to the right-hand
    bone (case-insensitive substring match, with fallbacks -- see
    find_prop_bone) so downstream ControlNet-Depth stylization draws a sword
    that is pinned to the character's grip every frame instead of a free
    floating blade the prompt has to guess the position of. The prop shares
    the body's beauty material and is automatically included in the depth
    material_override and the per-frame camera auto-framing bounds.
    --lock-facing (default: off, Phase 7.2) cancels root-bone yaw drift so
    the character's side-profile facing stays constant (locked to the
    first sampled frame's facing) across the whole sampled sequence -- see
    the "Facing lock" section below for how and why.
"""
import argparse
import math
import os
import re
import sys
import time

try:
    import bpy
    import bmesh
    from mathutils import Vector, Matrix
except ImportError:
    # Allows this module to be imported (e.g. for linting) outside Blender.
    # Actually running the pipeline this way is a hard error -- see main().
    bpy = None
    bmesh = None
    Vector = Matrix = None

try:
    import numpy as np
except ImportError:
    np = None  # Blender 5.1 bundles numpy; guarded for non-Blender imports only.


# ---------------------------------------------------------------------------
# Bone thickness heuristics
# ---------------------------------------------------------------------------
# Case-insensitive substring match against the bone name, checked in order
# (most specific first so e.g. "ForeArm" doesn't fall through to the
# generic "Arm" rule meant for upper arms). Tolerant of both SMPL/HumanML3D
# naming (Hips, Spine, LeftArm, LeftForeArm, LeftUpLeg, LeftLeg, ...) and
# generic rig naming (UpperArm, LowerArm, Thigh, Shin, ...). Any bone name
# that matches nothing falls back to a uniform "medium" thickness -- this
# heuristic never raises and never depends on a specific rig.
BONE_RULES = [
    (("finger", "thumb", "pinky", "index1", "index2", "index3",
      "middle1", "middle2", "middle3", "ring1", "ring2", "ring3"), 0.0, True),
    (("toe",), 0.0, True),
    (("head",), 1.9, False),
    (("neck",), 1.0, False),
    (("hip", "pelvis"), 2.1, False),
    (("spine", "chest", "torso", "abdomen", "ribcage"), 1.9, False),
    (("shoulder", "clavicle", "collar"), 0.9, False),
    (("forearm", "lower_arm", "lowerarm", "elbow"), 0.7, False),
    (("arm",), 0.85, False),                       # generic/upper arm fallback
    (("hand", "wrist"), 0.6, False),
    (("thigh", "upleg", "up_leg", "upperleg"), 1.4, False),
    (("shin", "calf", "lowleg", "downleg", "knee"), 1.05, False),
    (("leg",), 1.15, False),                       # generic leg fallback
    (("foot", "ankle"), 0.8, False),
]
DEFAULT_RADIUS_FACTOR = 1.0


def classify_bone(name):
    """Returns (radius_factor, skip) for a bone name. Never raises; unknown
    names fall back to a uniform capsule thickness (factor 1.0, not skipped)."""
    try:
        lname = name.lower()
        for keywords, factor, skip in BONE_RULES:
            if any(k in lname for k in keywords):
                return factor, skip
    except Exception:
        pass
    return DEFAULT_RADIUS_FACTOR, False


# ---------------------------------------------------------------------------
# Scene setup
# ---------------------------------------------------------------------------

def reset_scene():
    bpy.ops.wm.read_factory_settings(use_empty=True)


def import_bvh(bvh_path):
    """Imports the BVH as an armature with baked animation and returns the
    new armature object."""
    before = set(bpy.data.objects.keys())
    bpy.ops.import_anim.bvh(
        filepath=bvh_path,
        target="ARMATURE",
        global_scale=1.0,
        frame_start=1,
        use_fps_scale=False,
        update_scene_fps=True,
        update_scene_duration=True,
        axis_forward="-Z",
        axis_up="Y",
    )
    after = set(bpy.data.objects.keys())
    new_names = after - before
    armature_names = [n for n in new_names if bpy.data.objects[n].type == "ARMATURE"]
    if not armature_names:
        raise RuntimeError(f"BVH import produced no armature object (bvh={bvh_path})")
    return bpy.data.objects[armature_names[0]]


def build_body_mesh(armature_obj):
    """Builds a single mesh object giving the armature visible volume, one
    connected humanoid silhouette rather than floating segments:

    - one cylinder per (non-skipped) bone, extended ~15% beyond each end
      so neighboring segments overlap at joints;
    - an icosphere at each bone end (rounded capsule caps that double as
      joint balls where parent/child bones meet);
    - a bridging capsule wherever a child bone's head does not touch its
      parent's tail (common in BVH rigs at branch points, e.g. chest ->
      neck/shoulders, hips -> legs). The bridge is weighted to the PARENT
      bone: both of its endpoints (parent tail, child head) are rigid with
      respect to the parent's pose transform, so this deforms correctly.

    Everything is skinned via vertex groups + an Armature modifier so it
    follows the baked animation."""
    bones = [b for b in armature_obj.data.bones if b.length > 1e-5]
    lengths = [b.length for b in bones]
    avg_len = (sum(lengths) / len(lengths)) if lengths else 1.0
    base_radius = max(avg_len * 0.26, 1e-4)

    bm = bmesh.new()
    weights = {}  # bone_name -> list of vertex indices
    z_axis = Vector((0.0, 0.0, 1.0))

    def bone_radius(b):
        factor, skip = classify_bone(b.name)
        if skip:
            return None
        return max(base_radius * factor, base_radius * 0.15)

    def add_sphere(center, radius, weight_name):
        start_idx = len(bm.verts)
        bmesh.ops.create_icosphere(
            bm, subdivisions=1, radius=radius,
            matrix=Matrix.Translation(center),
        )
        bm.verts.ensure_lookup_table()
        weights.setdefault(weight_name, []).extend(range(start_idx, len(bm.verts)))

    def add_capsule(p0, p1, radius, weight_name, extend=0.15):
        direction_vec = p1 - p0
        length = direction_vec.length
        if length < 1e-5:
            add_sphere(p0, radius, weight_name)
            return
        direction = direction_vec.normalized()
        mid = (p0 + p1) / 2.0
        depth = length * (1.0 + 2.0 * extend)  # ~15% beyond each end
        rot = z_axis.rotation_difference(direction)
        matrix = Matrix.Translation(mid) @ rot.to_matrix().to_4x4()
        start_idx = len(bm.verts)
        bmesh.ops.create_cone(
            bm, cap_ends=True, cap_tris=False, segments=8,
            radius1=radius, radius2=radius, depth=depth, matrix=matrix,
        )
        bm.verts.ensure_lookup_table()
        weights.setdefault(weight_name, []).extend(range(start_idx, len(bm.verts)))
        # Rounded caps / joint balls at both ends.
        add_sphere(p0, radius, weight_name)
        add_sphere(p1, radius, weight_name)

    for b in bones:
        radius = bone_radius(b)
        if radius is None:
            continue
        add_capsule(b.head_local, b.tail_local, radius, b.name)

    # Bridge gaps between disconnected parent/child bones.
    for b in bones:
        parent = b.parent
        if parent is None:
            continue
        child_r = bone_radius(b)
        parent_r = bone_radius(parent)
        if child_r is None or parent_r is None:
            continue  # don't bridge into skipped bones (fingers/toes)
        gap_vec = b.head_local - parent.tail_local
        bridge_r = min(child_r, parent_r) * 0.9
        if gap_vec.length > bridge_r * 0.25:
            add_capsule(parent.tail_local, b.head_local, bridge_r,
                        parent.name, extend=0.05)

    if not weights:
        # Degenerate/unknown rig where every bone was skipped -- fall back
        # to a uniform capsule per bone regardless of name so we still
        # produce *some* volume rather than an empty mesh.
        for b in bones:
            add_capsule(b.head_local, b.tail_local, base_radius, b.name)

    me = bpy.data.meshes.new("Body")
    bm.to_mesh(me)
    bm.free()
    me.update()

    obj = bpy.data.objects.new("Body", me)
    bpy.context.collection.objects.link(obj)

    for name, indices in weights.items():
        vg = obj.vertex_groups.new(name=name)
        vg.add(indices, 1.0, "REPLACE")

    mod = obj.modifiers.new("Armature", "ARMATURE")
    mod.object = armature_obj
    mod.use_vertex_groups = True

    obj.parent = armature_obj
    obj.matrix_parent_inverse = armature_obj.matrix_world.inverted()

    return obj



# ---------------------------------------------------------------------------
# Optional weapon prop (Phase 6.2b)
# ---------------------------------------------------------------------------
# The BVH rigs seen in this pipeline are 22-joint HumanML3D/SMPL-named
# (Hips, Spine, ..., RightArm, RightForeArm, RightHand, ...), but the
# matching below is deliberately name-heuristic rather than hardcoded to
# that exact joint set, so it degrades gracefully on other rigs too.

def _bone_side_score(name, side):
    """True if `name` unambiguously reads as the given side ("right" or
    "left"), tolerating both full words (RightHand, Right_Hand) and short
    suffix/prefix conventions (hand.R, hand_R, R_Hand). Never raises;
    ambiguous or unmarked names return False for both sides."""
    ln = name.lower()
    other = "left" if side == "right" else "right"
    if other in ln:
        return False
    if side in ln:
        return True
    suffix = "r" if side == "right" else "l"
    if re.search(r"[._]" + suffix + r"\d*$", ln) or re.search(r"^" + suffix + r"[._]", ln):
        return True
    return False


def _bone_depth(bone):
    depth = 0
    p = bone.parent
    while p is not None:
        depth += 1
        p = p.parent
    return depth


def find_prop_bone(armature_obj):
    """Finds the bone to attach a hand-held prop to. Preference order:
    1. a right-hand/wrist-named bone (case-insensitive substring, side-
       aware -- see _bone_side_score);
    2. failing that, the deepest (most distal) bone of the right arm chain
       (RightForeArm et al) so the prop still lands roughly at the wrist;
    3. failing that, the same two steps on the left side.
    Returns (bone, side_label) or (None, None) if the rig has nothing that
    looks like an arm at all -- callers must treat that as "skip the prop,
    don't crash", never raise from here."""
    try:
        bones = list(armature_obj.data.bones)
    except Exception:
        return None, None
    if not bones:
        return None, None

    hand_keywords = ("hand", "wrist")
    arm_keywords = ("hand", "wrist", "forearm", "arm", "elbow", "shoulder", "clavicle", "collar")

    for side in ("right", "left"):
        hand_matches = [b for b in bones
                         if any(k in b.name.lower() for k in hand_keywords)
                         and _bone_side_score(b.name, side)]
        if hand_matches:
            hand_only = [b for b in hand_matches if "hand" in b.name.lower()]
            pool = hand_only if hand_only else hand_matches
            return max(pool, key=_bone_depth), side

    for side in ("right", "left"):
        arm_matches = [b for b in bones
                        if any(k in b.name.lower() for k in arm_keywords)
                        and _bone_side_score(b.name, side)]
        if arm_matches:
            return max(arm_matches, key=_bone_depth), side

    return None, None


def compute_arm_length(bone):
    """Approximate limb reach by summing bone lengths from the matched bone
    up through its forearm/upper-arm/shoulder ancestors, stopping at the
    torso/neck/hip root. Falls back to a multiple of the bone's own length
    if the chain is degenerate (e.g. a single-bone arm or a rig where the
    parent chain doesn't resolve as expected) -- this must never raise or
    return zero, since it directly sizes the sword mesh."""
    stop_words = ("spine", "chest", "torso", "abdomen", "ribcage", "neck",
                  "head", "hip", "pelvis")
    total = 0.0
    b = bone
    steps = 0
    try:
        while b is not None and steps < 6:
            total += b.length
            parent = b.parent
            if parent is None:
                break
            if any(k in parent.name.lower() for k in stop_words):
                break
            b = parent
            steps += 1
    except Exception:
        pass
    if total < 1e-4:
        total = max(bone.length * 3.0, 0.1)
    return total


def build_sword_mesh(arm_length, name="Sword"):
    """Builds a greatsword-proportioned mesh authored directly in the
    target bone's local frame: origin at the bone's tail (see
    attach_prop_to_bone), blade extending along local +Y -- a bone's local
    Y axis runs head->tail by Blender convention, i.e. away from the
    forearm for a hand/wrist bone, so no extra reorientation is needed.
    Proportions are all derived from `arm_length` (the reach computed by
    compute_arm_length) so the sword scales sensibly with any rig."""
    bm = bmesh.new()
    z_axis = Vector((0.0, 0.0, 1.0))

    def add_segment(p0, p1, r0, r1, segments=8):
        direction_vec = p1 - p0
        length = direction_vec.length
        if length < 1e-6:
            return
        direction = direction_vec.normalized()
        mid = (p0 + p1) / 2.0
        rot = z_axis.rotation_difference(direction)
        matrix = Matrix.Translation(mid) @ rot.to_matrix().to_4x4()
        bmesh.ops.create_cone(
            bm, cap_ends=True, cap_tris=False, segments=segments,
            radius1=r0, radius2=r1, depth=length, matrix=matrix,
        )

    L = arm_length
    pommel_len = L * 0.035
    grip_len = L * 0.14
    guard_thick = L * 0.02
    blade_len = L * 1.35  # within the requested 1.2-1.5x arm-length range

    grip_r = L * 0.028
    pommel_r = L * 0.045
    guard_bar_r = L * 0.014
    guard_half_width = L * 0.16
    blade_base_r = L * 0.05

    y0 = -(pommel_len + grip_len)
    y1 = -grip_len          # grip start (pommel side)
    y2 = 0.0                # grip end == bone tail == attach origin
    y3 = guard_thick        # blade root, just past the crossguard

    # Pommel cap tapering into the grip.
    add_segment(Vector((0, y0, 0)), Vector((0, y1, 0)), pommel_r, grip_r)
    # Grip (round cylinder).
    add_segment(Vector((0, y1, 0)), Vector((0, y2, 0)), grip_r, grip_r)
    # Crossguard: thin horizontal bar centered on the grip/blade junction.
    add_segment(Vector((-guard_half_width, guard_thick / 2.0, 0)),
                Vector((guard_half_width, guard_thick / 2.0, 0)),
                guard_bar_r, guard_bar_r, segments=6)
    # Blade: tapered to a point, flattened (4-sided) cross-section.
    add_segment(Vector((0, y3, 0)), Vector((0, y3 + blade_len, 0)),
                blade_base_r, 0.0, segments=4)

    me = bpy.data.meshes.new(name)
    bm.to_mesh(me)
    bm.free()
    me.update()

    obj = bpy.data.objects.new(name, me)
    return obj


def attach_prop_to_bone(prop_obj, armature_obj, bone):
    """Rigidly follows `bone`'s pose every frame via a Child Of constraint
    (constraint-based, not parent_type='BONE', so the object's own matrix
    stays simple and fully explicit). Child Of composes the bone's posed
    matrix (head-based, local Y = head->tail) with the object's own
    matrix_basis, so offsetting the object's location by the bone's own
    length along local Y places the mesh's authored origin (y=0, see
    build_sword_mesh) at the bone's TAIL rather than its head -- i.e. at
    the wrist-to-fingertip end of a hand bone (or at the wrist itself, if
    we fell back to attaching to the forearm), which is where a gripped
    weapon's hilt naturally sits."""
    prop_obj.location = Vector((0.0, bone.length, 0.0))
    prop_obj.rotation_euler = (0.0, 0.0, 0.0)
    prop_obj.scale = (1.0, 1.0, 1.0)
    con = prop_obj.constraints.new(type="CHILD_OF")
    con.target = armature_obj
    con.subtarget = bone.name
    con.inverse_matrix = Matrix.Identity(4)


def build_and_attach_prop(armature_obj, prop_name, beauty_mat):
    """Top-level entry point for --prop. Returns the prop object, or None if
    no prop was requested or no suitable bone could be found -- this never
    raises for a "no bone found" rig; it prints a warning and returns None
    so main() renders without the prop rather than crashing."""
    if not prop_name:
        return None
    if prop_name != "sword":
        print(f"[Blender] WARNING: unknown --prop '{prop_name}'; ignoring.")
        return None

    bone, side = find_prop_bone(armature_obj)
    if bone is None:
        print("[Blender] WARNING: no hand/wrist/arm bone found on this rig; "
              "rendering without the sword prop.")
        return None

    arm_length = compute_arm_length(bone)
    prop_obj = build_sword_mesh(arm_length)
    prop_obj.data.materials.append(beauty_mat)
    bpy.context.collection.objects.link(prop_obj)
    attach_prop_to_bone(prop_obj, armature_obj, bone)

    print(f"[Blender] Prop 'sword' bound to bone '{bone.name}' ({side} side), "
          f"arm_length={arm_length:.4f}, blade_length={arm_length * 1.35:.4f}")
    return prop_obj


def make_materials():
    """Returns (beauty_material, depth_material, depth_maprange_node).

    beauty_material: flat matte gray Principled BSDF, lit by scene lights.
    depth_material: Camera Data (View Z Depth) -> Map Range -> Emission.
    Applied via view_layer.material_override so the mesh's own material
    slot doesn't need to change between passes.
    """
    beauty = bpy.data.materials.new("BodyBeauty")
    bsdf = beauty.node_tree.nodes.get("Principled BSDF")
    if bsdf is not None:
        bsdf.inputs["Base Color"].default_value = (0.55, 0.55, 0.58, 1.0)
        if "Roughness" in bsdf.inputs:
            bsdf.inputs["Roughness"].default_value = 0.85

    depth = bpy.data.materials.new("BodyDepth")
    nt = depth.node_tree
    nt.nodes.clear()
    camdata = nt.nodes.new("ShaderNodeCameraData")
    maprange = nt.nodes.new("ShaderNodeMapRange")
    maprange.clamp = True
    # From Min/Max are overwritten per frame in render_all with the
    # figure's actual near/far depth for that frame, so the figure spans
    # the full ControlNet-Depth brightness convention every frame: nearest
    # surface -> 1.0 (white), farthest surface -> 0.3 (dim gray, still
    # clearly separated from the pure-black background applied by
    # blacken_depth_background).
    maprange.inputs["From Min"].default_value = 0.0
    maprange.inputs["From Max"].default_value = 10.0
    maprange.inputs["To Min"].default_value = 1.0   # near -> white
    maprange.inputs["To Max"].default_value = 0.3   # far -> dim gray
    combine = nt.nodes.new("ShaderNodeCombineColor")
    emission = nt.nodes.new("ShaderNodeEmission")
    output = nt.nodes.new("ShaderNodeOutputMaterial")
    nt.links.new(camdata.outputs["View Z Depth"], maprange.inputs["Value"])
    nt.links.new(maprange.outputs["Result"], combine.inputs["Red"])
    nt.links.new(maprange.outputs["Result"], combine.inputs["Green"])
    nt.links.new(maprange.outputs["Result"], combine.inputs["Blue"])
    nt.links.new(combine.outputs["Color"], emission.inputs["Color"])
    nt.links.new(emission.outputs["Emission"], output.inputs["Surface"])

    return beauty, depth, maprange


def setup_lighting():
    """Simple key + fill sun lights plus a soft world ambient, enough for
    the flat-gray beauty pass to read as a neutral shaded figure."""
    bpy.ops.object.light_add(type="SUN", location=(0, 0, 0))
    key = bpy.context.object
    key.data.energy = 3.0
    key.rotation_euler = (math.radians(55), 0, math.radians(35))

    bpy.ops.object.light_add(type="SUN", location=(0, 0, 0))
    fill = bpy.context.object
    fill.data.energy = 1.0
    fill.rotation_euler = (math.radians(-55), 0, math.radians(-145))

    world = bpy.context.scene.world or bpy.data.worlds.new("World")
    bpy.context.scene.world = world
    if world.node_tree:
        bg = world.node_tree.nodes.get("Background")
        if bg is not None:
            bg.inputs["Color"].default_value = (0.5, 0.5, 0.55, 1.0)
            bg.inputs["Strength"].default_value = 0.35


# ---------------------------------------------------------------------------
# Frame sampling & camera framing
# ---------------------------------------------------------------------------

def get_action_frame_range(armature_obj, scene):
    """The imported BVH's actual keyframe range. Deliberately reads this
    from the action itself rather than scene.frame_start/frame_end, which
    have been observed to disagree with the real keyframe range after BVH
    import (e.g. reporting a scene end far past the last real keyframe)."""
    try:
        action = armature_obj.animation_data.action
        if action is not None:
            fr = action.frame_range
            return int(round(fr[0])), int(round(fr[1]))
    except Exception:
        pass
    return scene.frame_start, scene.frame_end


def sample_frames(start, end, n):
    n = max(1, n)
    if n == 1 or end <= start:
        return [start] * n
    return [int(round(start + i * (end - start) / (n - 1))) for i in range(n)]


# ---------------------------------------------------------------------------
# Facing lock (Phase 7.2)
# ---------------------------------------------------------------------------
# MoMask-generated motions often carry root yaw (the whole body slowly
# turning over the clip, sometimes a lot on turning/spinning attacks). Left
# alone, this reads as the character's side profile flipping toward front-
# or back-on for some sampled frames, which then confuses the downstream
# SDXL stylization pass into drawing an inconsistent facing across the
# sprite sheet. --lock-facing cancels this: for every SAMPLED frame, the
# root ("Hips") bone's current heading is measured and the whole armature
# OBJECT (not the individual pose bones) is counter-rotated around the
# world vertical axis, through the bone's own world-space position that
# frame, so the heading stays pinned to whatever it was on the first
# sampled frame.
#
# Rotating the OBJECT rather than the pose bones is deliberate: it composes
# for free with everything else already in the scene graph --
#   - the body mesh is parented to the armature object and skinned via an
#     Armature modifier that deforms in the armature's own local/object
#     space, so a change to the armature object's matrix_world propagates
#     through ordinary Blender parenting math without being double-applied;
#   - the sword prop is attached via a Child Of constraint targeting a bone
#     of this same armature, so it inherits the correction transparently
#     and stays rigidly gripped;
#   - it runs in main() BEFORE compute_bounds/setup_camera, so per-frame
#     camera auto-framing sees the corrected pose, not the original one.
#
# Only yaw (rotation about the global Z/vertical axis) is cancelled --
# pitch/roll (leaning, crouching) are left untouched since those aren't
# what breaks the side-profile read.


def find_root_bone(armature_obj):
    """Finds the skeleton's root motion bone (HumanML3D/SMPL: "Hips") to
    measure facing from. Preference order: (1) an actual armature root (no
    parent) whose name reads as hip/pelvis/root; (2) any actual armature
    root if there's exactly one; (3) any bone anywhere in the rig with a
    matching name; (4) give up. Returns a PoseBone or None -- callers must
    treat None as "skip the lock, don't crash", never raise from here."""
    try:
        pose_bones = list(armature_obj.pose.bones)
    except Exception:
        return None
    if not pose_bones:
        return None

    roots = [b for b in pose_bones if b.parent is None]
    keywords = ("hip", "pelvis", "root")

    named_roots = [b for b in roots if any(k in b.name.lower() for k in keywords)]
    if named_roots:
        return named_roots[0]
    if len(roots) == 1:
        return roots[0]

    named_any = [b for b in pose_bones if any(k in b.name.lower() for k in keywords)]
    if named_any:
        return named_any[0]
    if roots:
        return roots[0]
    return None


def _ground_yaw(world_matrix, local_axis=Vector((1.0, 0.0, 0.0))):
    """Yaw (radians, world XY plane) of `local_axis` as carried by
    world_matrix's rotation. Any fixed bone-local axis works here -- this
    angle is only ever compared to itself at other frames (relative
    drift), never treated as an absolute "this is anatomically forward"
    direction, so there's no dependency on the BVH exporter's bone-axis
    convention. Returns None if the axis has rotated to point (near)
    straight up/down, where a ground-plane heading is ambiguous."""
    direction = world_matrix.to_3x3() @ local_axis
    horiz = Vector((direction.x, direction.y))
    if horiz.length < 1e-6:
        return None
    return math.atan2(horiz.y, horiz.x)


def compute_facing_lock(armature_obj, root_bone_name, frames, scene):
    """Computes, for each sampled frame, the world-space delta rotation
    that cancels that frame's root-bone yaw drift relative to the FIRST
    sampled frame (frames[0]'s facing becomes the locked reference).
    Returns {frame: Matrix} -- a delta still needing to be premultiplied
    against the armature's ORIGINAL matrix_world by the caller, see
    make_facing_lock_applier -- or {} on any failure (bone not found,
    degenerate rig, etc.). Callers must treat {} as "render without the
    lock"; this never raises."""
    if not root_bone_name or not frames:
        return {}

    base_matrix_world = armature_obj.matrix_world.copy()
    corrections = {}
    ref_yaw = None

    try:
        for f in frames:
            scene.frame_set(f)
            bpy.context.view_layer.update()

            pose_bone = armature_obj.pose.bones.get(root_bone_name)
            if pose_bone is None:
                return {}

            world_matrix = base_matrix_world @ pose_bone.matrix
            yaw = _ground_yaw(world_matrix)
            if yaw is None:
                corrections[f] = Matrix.Identity(4)
                continue

            if ref_yaw is None:
                ref_yaw = yaw

            delta = ref_yaw - yaw
            pivot = world_matrix.translation.copy()
            corrections[f] = (Matrix.Translation(pivot)
                               @ Matrix.Rotation(delta, 4, "Z")
                               @ Matrix.Translation(-pivot))
    except Exception as exc:
        print(f"[Blender] WARNING: facing-lock computation raised {exc!r}; "
              f"rendering without facing lock.")
        return {}

    return corrections


def make_facing_lock_applier(armature_obj, corrections):
    """Returns a callable(f) that resets the armature object's matrix_world
    to its ORIGINAL transform corrected by frame f's facing-lock delta (or
    just the original transform if f has no entry). Always rebuilding from
    the captured original matrix_world means repeated/out-of-order calls
    never accumulate. Returns None if `corrections` is empty (nothing to
    apply, e.g. lock disabled or computation failed)."""
    if not corrections:
        return None
    base_matrix_world = armature_obj.matrix_world.copy()

    def apply(f):
        correction = corrections.get(f)
        armature_obj.matrix_world = (correction @ base_matrix_world
                                      if correction is not None else base_matrix_world)

    return apply


def compute_bounds(scene, mesh_objs, frames, facing_lock_apply=None):
    """Per-frame world-space bounding box of the (deformed) mesh objects.
    Uses each evaluated mesh's actual vertices rather than object.bound_box,
    since bound_box has been observed to NOT reflect Armature-modifier
    deformation in this Blender version. `mesh_objs` may be a single object
    or a list -- when it's the body mesh plus an attached prop, the union
    of both is returned so the prop (e.g. a sword) factors into camera
    auto-framing and never exits frame.

    Returns {frame: (fmin, fmax)} -- deliberately kept per-frame (not just
    a single global union) so the camera can recenter on the character
    each frame and ignore accumulated root-motion translation when sizing
    the ortho frustum (see setup_camera).

    `facing_lock_apply`, if given (see make_facing_lock_applier), is called
    for each frame right after posing it, BEFORE bounds are read, so the
    bounds -- and therefore the camera framing derived from them -- reflect
    the facing-locked pose rather than the original one."""
    if hasattr(mesh_objs, "evaluated_get"):
        mesh_objs = [mesh_objs]
    bounds_by_frame = {}

    for f in frames:
        scene.frame_set(f)
        bpy.context.view_layer.update()
        if facing_lock_apply is not None:
            facing_lock_apply(f)
            bpy.context.view_layer.update()
        dg = bpy.context.evaluated_depsgraph_get()
        worlds = []
        for mesh_obj in mesh_objs:
            obj_eval = mesh_obj.evaluated_get(dg)
            me_eval = obj_eval.to_mesh()
            n = len(me_eval.vertices)
            if n == 0:
                obj_eval.to_mesh_clear()
                continue
            co = np.empty(n * 3, dtype=np.float32)
            me_eval.vertices.foreach_get("co", co)
            co = co.reshape(-1, 3)
            mw = np.array(obj_eval.matrix_world)
            world = (mw[:3, :3] @ co.T).T + mw[:3, 3]
            obj_eval.to_mesh_clear()
            worlds.append(world)

        if not worlds:
            bounds_by_frame[f] = (Vector((0, 0, 0)), Vector((0, 0, 0)))
            continue

        combined = np.concatenate(worlds, axis=0)
        fmin, fmax = combined.min(axis=0), combined.max(axis=0)
        bounds_by_frame[f] = (Vector((float(fmin[0]), float(fmin[1]), float(fmin[2]))),
                               Vector((float(fmax[0]), float(fmax[1]), float(fmax[2]))))

    return bounds_by_frame


def setup_camera(scene, bounds_by_frame, margin=1.2):
    """Orthographic side-view camera, auto-framed around the animated
    figure. Which horizontal world axis becomes "screen width" vs. "camera
    depth" is chosen from the character's own per-frame silhouette extent
    (the axis with more spread within a single pose -- not the raw world
    range across frames) rather than assumed from BVH convention, so this
    adapts to whatever axis convention the source rig ends up in after
    Blender's BVH-import axis remap.

    Root motion (e.g. a walking clip's forward translation accumulated
    over many frames) is deliberately NOT used to size the frustum: sizing
    ortho_scale to fit the *entire* walk's cumulative drift would shrink
    the character to a speck for anything but a stationary animation, and
    game sprite sheets need an in-place, consistently-sized figure per
    frame (locomotion is handled by moving the game object, not by the
    sprite drifting across its own frame). Instead, ortho_scale is sized
    from the character's own local silhouette (max per-frame extent), and
    the camera is *recentered* on the character's per-frame bounding-box
    center during rendering (see render_all/get_camera_loc_for_frame) --
    a fixed-size virtual camera on a dolly track, panning to follow the
    subject, which is the standard technique for turntable/cycle renders.
    """
    up_i = 2  # Blender world is always Z-up.
    frames = list(bounds_by_frame.keys())

    # Per-frame LOCAL extents (ignores drift): how big is the character's
    # own silhouette in a single pose, on each axis.
    local_extent = [0.0, 0.0, 0.0]
    for f in frames:
        fmin, fmax = bounds_by_frame[f]
        for i in range(3):
            local_extent[i] = max(local_extent[i], fmax[i] - fmin[i])

    # Screen-axis choice, two signals in priority order:
    # 1. Root travel: if the clip locomotes (bbox center drifts more than
    #    ~25% of the figure's height), the travel direction IS the
    #    side-scroller axis -- a walking character must be seen moving
    #    across the screen, not toward the camera. This wins over the
    #    extent heuristic because a humanoid's shoulder width can rival
    #    its stride depth in any single pose, making per-pose extents an
    #    unreliable indicator of facing.
    # 2. Otherwise (stationary clips, e.g. an attack), the horizontal axis
    #    with the larger per-pose silhouette extent is treated as the
    #    action plane and becomes the screen axis.
    first_min, first_max = bounds_by_frame[frames[0]]
    last_min, last_max = bounds_by_frame[frames[-1]]
    travel = [abs(((last_min[i] + last_max[i]) - (first_min[i] + first_max[i])) / 2.0)
              for i in range(3)]
    height = max(local_extent[up_i], 1e-4)
    horizontal_travel = max(travel[0], travel[1])

    if horizontal_travel > 0.25 * height:
        screen_i = 0 if travel[0] >= travel[1] else 1
    else:
        screen_i = 0 if local_extent[0] >= local_extent[1] else 1
    depth_i = 1 - screen_i

    width = local_extent[screen_i]
    height = local_extent[up_i]
    ortho_scale = max(width, height, 0.5) * margin  # ~10% margin per side

    half_depth = max(local_extent[depth_i] / 2.0, 0.05)
    distance = half_depth * 3.0 + max(ortho_scale, 1.0) + 2.0

    # Fixed viewing direction: a camera looking straight down -depth_axis
    # has the same rotation regardless of where along screen/up it sits,
    # so rotation can be computed once from the axis convention alone.
    direction = Vector((0.0, 0.0, 0.0))
    direction[depth_i] = -1.0

    # Initial placement centered on frame[0]'s bbox; per-frame recentering
    # happens in render_all via get_camera_loc_for_frame().
    f0min, f0max = bounds_by_frame[frames[0]]
    f0center = (f0min + f0max) / 2.0
    cam_loc = Vector((0.0, 0.0, 0.0))
    cam_loc[screen_i] = f0center[screen_i]
    cam_loc[up_i] = f0center[up_i]
    cam_loc[depth_i] = f0center[depth_i] + distance

    bpy.ops.object.camera_add(location=cam_loc)
    cam = bpy.context.object
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = ortho_scale
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()

    pad = half_depth * 0.1 + 0.01
    near = distance - half_depth - pad
    far = distance + half_depth + pad
    cam.data.clip_start = max(0.01, near * 0.5)
    cam.data.clip_end = far * 1.5

    scene.camera = cam

    def get_camera_loc_for_frame(f):
        fmin, fmax = bounds_by_frame[f]
        fcenter = (fmin + fmax) / 2.0
        loc = Vector((0.0, 0.0, 0.0))
        loc[screen_i] = fcenter[screen_i]
        loc[up_i] = fcenter[up_i]
        loc[depth_i] = fcenter[depth_i] + distance
        return loc

    def get_depth_range_for_frame(f):
        """Figure's exact near/far camera-space depth for a frame, given
        the camera recentered by get_camera_loc_for_frame (which puts the
        figure's depth-axis bbox center exactly `distance` in front of
        the camera). Used to normalize the depth map per frame so the
        figure always spans the full depth-brightness range."""
        fmin, fmax = bounds_by_frame[f]
        half = max((fmax[depth_i] - fmin[depth_i]) / 2.0, 0.01)
        return distance - half, distance + half

    return cam, near, far, screen_i, depth_i, get_camera_loc_for_frame, get_depth_range_for_frame


# ---------------------------------------------------------------------------
# Rendering
# ---------------------------------------------------------------------------

def blacken_depth_background(path):
    """Post-process a rendered depth PNG: composite the transparent
    background (alpha<0.5, i.e. no geometry) to pure black (far, per the
    ControlNet-Depth convention: near = bright, far = dark) and force full
    opacity. Figure pixels (the grayscale depth gradient) are left as-is."""
    img = bpy.data.images.load(path, check_existing=False)
    try:
        w, h = img.size
        n = w * h * 4
        arr = np.empty(n, dtype=np.float32)
        img.pixels.foreach_get(arr)
        arr = arr.reshape(h, w, 4)

        bg_mask = arr[..., 3] < 0.5
        arr[bg_mask, 0:3] = 0.0
        arr[..., 3] = 1.0

        img.pixels.foreach_set(arr.reshape(-1))
        img.filepath_raw = path
        img.file_format = "PNG"
        img.save()
    finally:
        bpy.data.images.remove(img)


def render_all(scene, cam, body_obj, depth_mat, depth_maprange, frames, out_dir,
               get_camera_loc_for_frame, get_depth_range_for_frame,
               facing_lock_apply=None):
    view_layer = bpy.context.view_layer
    for idx, f in enumerate(frames):
        scene.frame_set(f)
        bpy.context.view_layer.update()
        if facing_lock_apply is not None:
            facing_lock_apply(f)
            bpy.context.view_layer.update()
        # Recenter the (fixed-size, fixed-rotation) camera on this frame's
        # character position -- see setup_camera for why.
        cam.location = get_camera_loc_for_frame(f)

        view_layer.material_override = None
        beauty_path = os.path.join(out_dir, f"frame_{idx:04d}.png")
        scene.render.filepath = beauty_path
        bpy.ops.render.render(write_still=True)

        # Normalize this frame's depth over the figure's own depth range.
        near_f, far_f = get_depth_range_for_frame(f)
        depth_maprange.inputs["From Min"].default_value = near_f
        depth_maprange.inputs["From Max"].default_value = far_f

        view_layer.material_override = depth_mat
        depth_path = os.path.join(out_dir, f"depth_{idx:04d}.png")
        scene.render.filepath = depth_path
        bpy.ops.render.render(write_still=True)
        blacken_depth_background(depth_path)

        print(f"[Blender] Frame {idx + 1}/{len(frames)} (bvh frame {f}) rendered.")

    view_layer.material_override = None


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

def parse_args():
    argv = sys.argv
    if "--" in argv:
        argv = argv[argv.index("--") + 1:]
    else:
        argv = []
    p = argparse.ArgumentParser(
        description="Render a BVH animation to beauty+depth control image frames."
    )
    p.add_argument("bvh_path")
    p.add_argument("output_dir")
    p.add_argument("--frames", type=int, default=16)
    p.add_argument("--res", type=int, default=1024)
    p.add_argument("--prop", choices=["sword"], default=None,
                    help="Attach a weapon prop to the right-hand bone "
                         "(default: none).")
    p.add_argument("--lock-facing", dest="lock_facing", action="store_true",
                    default=False,
                    help="Cancel root-bone yaw drift so the character's "
                         "side-profile facing stays constant (locked to "
                         "the first sampled frame's facing) across all "
                         "sampled frames (default: off, current free-yaw "
                         "behavior).")
    return p.parse_args(argv)


def main():
    if bpy is None:
        sys.stderr.write(
            "[Blender] ERROR: bpy is not available -- this script must be run "
            "inside Blender, not with a plain python3 interpreter.\n"
            "Invoke as:\n"
            "  /Applications/Blender.app/Contents/MacOS/Blender -b -P "
            "AIPipeline/src/blender_render.py -- <bvh_path> <output_dir> "
            "[--frames N] [--res R]\n"
        )
        sys.exit(1)

    args = parse_args()
    t0 = time.time()

    bvh_path = os.path.abspath(args.bvh_path)
    out_dir = os.path.abspath(args.output_dir)
    if not os.path.isfile(bvh_path):
        sys.stderr.write(f"[Blender] ERROR: BVH file not found: {bvh_path}\n")
        sys.exit(1)
    os.makedirs(out_dir, exist_ok=True)

    print(f"[Blender] Loading BVH: {bvh_path}")
    reset_scene()
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.resolution_x = args.res
    scene.render.resolution_y = args.res
    scene.render.resolution_percentage = 100
    scene.render.film_transparent = True
    scene.render.image_settings.file_format = "PNG"
    scene.render.image_settings.color_mode = "RGBA"
    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    try:
        scene.eevee.taa_render_samples = 32
    except Exception:
        pass

    armature_obj = import_bvh(bvh_path)
    print(f"[Blender] Imported armature with {len(armature_obj.data.bones)} bones")

    body_obj = build_body_mesh(armature_obj)
    print(f"[Blender] Built body mesh: {len(body_obj.data.vertices)} verts, "
          f"{len(body_obj.vertex_groups)} bone groups")

    beauty_mat, depth_mat, depth_maprange = make_materials()
    body_obj.data.materials.append(beauty_mat)

    prop_obj = None
    if args.prop:
        try:
            prop_obj = build_and_attach_prop(armature_obj, args.prop, beauty_mat)
        except Exception as exc:
            print(f"[Blender] WARNING: prop attachment raised {exc!r}; "
                  f"continuing without prop.")
            prop_obj = None

    setup_lighting()

    start_f, end_f = get_action_frame_range(armature_obj, scene)
    frames = sample_frames(start_f, end_f, args.frames)
    print(f"[Blender] Animation range {start_f}-{end_f}, "
          f"sampling {len(frames)} frames: {frames}")

    facing_lock_apply = None
    if args.lock_facing:
        try:
            root_bone = find_root_bone(armature_obj)
        except Exception as exc:
            print(f"[Blender] WARNING: root-bone lookup raised {exc!r}; "
                  f"rendering without facing lock.")
            root_bone = None
        if root_bone is None:
            print("[Blender] WARNING: --lock-facing requested but no "
                  "root/hips bone could be identified on this rig; "
                  "rendering without facing lock.")
        else:
            corrections = compute_facing_lock(armature_obj, root_bone.name, frames, scene)
            facing_lock_apply = make_facing_lock_applier(armature_obj, corrections)
            if facing_lock_apply is None:
                print("[Blender] WARNING: facing-lock correction computation "
                      "produced no usable data; rendering without facing lock.")
            else:
                print(f"[Blender] Facing lock enabled: root bone "
                      f"'{root_bone.name}', reference facing = frame "
                      f"{frames[0]} (first sampled frame).")

    mesh_objs = [body_obj] + ([prop_obj] if prop_obj is not None else [])
    bounds_by_frame = compute_bounds(scene, mesh_objs, frames, facing_lock_apply)
    (cam, near, far, screen_i, depth_i,
     get_camera_loc_for_frame, get_depth_range_for_frame) = setup_camera(scene, bounds_by_frame)
    axis_names = "XYZ"
    print(f"[Blender] Camera framed: ortho_scale={cam.data.ortho_scale:.2f} "
          f"near={near:.2f} far={far:.2f} "
          f"screen_axis={axis_names[screen_i]} depth_axis={axis_names[depth_i]}")

    render_all(scene, cam, body_obj, depth_mat, depth_maprange, frames, out_dir,
               get_camera_loc_for_frame, get_depth_range_for_frame,
               facing_lock_apply)

    elapsed = time.time() - t0
    print(f"[Blender] Render complete: {len(frames)} beauty + {len(frames)} depth "
          f"frames in {elapsed:.1f}s ({elapsed / max(len(frames), 1):.2f}s/frame) "
          f"-> {out_dir}")


if __name__ == "__main__":
    main()
