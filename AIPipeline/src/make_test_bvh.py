"""
Generates a small, self-contained, valid humanoid .bvh file for testing the
Blender render stage (blender_render.py) without depending on the MoMask
text-to-motion stage (handled separately, in parallel, by another agent).

Pure Python, no bpy / no external deps -- can be run standalone:

    python3 AIPipeline/src/make_test_bvh.py [output_path]

Produces a ~21-joint HumanML3D/SMPL-style skeleton (Hips, Spine, Spine1,
Neck, Head, Left/RightShoulder, Left/RightArm, Left/RightForeArm,
Left/RightHand, Left/RightUpLeg, Left/RightLeg, Left/RightFoot,
Left/RightToeBase) with 60 frames (~2s @ 30fps) of a walk cycle: root
translates forward along +Z with a small vertical bob, arms and legs swing
in a simple opposing sine pattern, spine/head sway slightly. This is
intentionally simple -- it exists to exercise the render pipeline
end-to-end (import -> body mesh -> camera framing -> render), not to be a
believable mocap take.

Joint naming intentionally matches the SMPL/HumanML3D convention MoMask
outputs (see .planning/phases/6.2/SUMMARY.md), so this test asset is a
reasonable stand-in for real MoMask output as far as blender_render.py's
bone-thickness heuristics are concerned.
"""
import math
import os
import sys

FPS = 30
NUM_FRAMES = 60
FRAME_TIME = 1.0 / FPS

# BVH convention used here: Y-up, +Z forward (matches Blender's BVH importer
# defaults: axis_up='Y', axis_forward='-Z') and typical mocap/HumanML3D BVH
# exports. Units are arbitrary "BVH units" roughly analogous to centimeters
# (total standing height ~168 units).


class Joint:
    def __init__(self, name, offset, channels, children=None, end_offset=None):
        self.name = name
        self.offset = offset  # (x, y, z) relative to parent
        self.channels = channels  # list of channel names, in order
        self.children = children or []
        self.end_offset = end_offset  # optional End Site offset (leaf joints)


def build_skeleton():
    """Builds the joint hierarchy with rest-pose offsets (a T/A-pose-ish
    humanoid, roughly 168 units tall)."""

    root_chans = ["Xposition", "Yposition", "Zposition", "Zrotation", "Xrotation", "Yrotation"]
    rot_chans = ["Zrotation", "Xrotation", "Yrotation"]

    def leg(prefix, side_x):
        return Joint(f"{prefix}UpLeg", (side_x, -8.0, 0.0), rot_chans, [
            Joint(f"{prefix}Leg", (0.0, -42.0, 0.0), rot_chans, [
                Joint(f"{prefix}Foot", (0.0, -40.0, 0.0), rot_chans, [
                    Joint(f"{prefix}ToeBase", (0.0, -6.0, 12.0), rot_chans,
                          end_offset=(0.0, -1.0, 5.0)),
                ]),
            ]),
        ])

    def arm(prefix, side_x):
        # Arms hang down at the sides (natural standing/walking rest pose)
        # rather than a T-pose. This matters for more than looks: a T-pose
        # (arms straight out sideways) makes the character's left-right
        # extent enormous and roughly constant regardless of pose, which
        # dominates blender_render.py's per-frame silhouette-extent camera
        # heuristic and biases it toward picking a front/back view instead
        # of a true side view. Hanging arms keep the rest-pose left-right
        # extent close to shoulder width, so the walk cycle's forward/back
        # swing (arms + legs) is what actually dominates, and the side
        # view auto-framing picks the correct axis.
        return Joint(f"{prefix}Shoulder", (side_x * 4.0, 2.0, 0.0), rot_chans, [
            Joint(f"{prefix}Arm", (side_x * 2.0, -12.0, 0.0), rot_chans, [
                Joint(f"{prefix}ForeArm", (side_x * 1.0, -22.0, 0.0), rot_chans, [
                    Joint(f"{prefix}Hand", (side_x * 0.5, -18.0, 0.0), rot_chans,
                          end_offset=(0.0, -7.0, 0.0)),
                ]),
            ]),
        ])

    hips = Joint("Hips", (0.0, 90.0, 0.0), root_chans, [
        Joint("Spine", (0.0, 12.0, 0.0), rot_chans, [
            Joint("Spine1", (0.0, 12.0, 0.0), rot_chans, [
                Joint("Neck", (0.0, 10.0, 0.0), rot_chans, [
                    Joint("Head", (0.0, 8.0, 0.0), rot_chans,
                          end_offset=(0.0, 20.0, 0.0)),
                ]),
                arm("Left", +1.0),
                arm("Right", -1.0),
            ]),
        ]),
        leg("Left", +8.0),
        leg("Right", -8.0),
    ])
    return hips


def flatten_order(joint, out):
    """Depth-first traversal producing the channel order BVH expects."""
    out.append(joint)
    for c in joint.children:
        flatten_order(c, out)
    return out


def write_hierarchy(joint, depth, lines):
    indent = "  " * depth
    tag = "ROOT" if depth == 0 else "JOINT"
    lines.append(f"{indent}{tag} {joint.name}")
    lines.append(f"{indent}{{")
    lines.append(f"{indent}  OFFSET {joint.offset[0]:.4f} {joint.offset[1]:.4f} {joint.offset[2]:.4f}")
    lines.append(f"{indent}  CHANNELS {len(joint.channels)} {' '.join(joint.channels)}")
    for c in joint.children:
        write_hierarchy(c, depth + 1, lines)
    if joint.end_offset is not None:
        lines.append(f"{indent}  End Site")
        lines.append(f"{indent}  {{")
        lines.append(f"{indent}    OFFSET {joint.end_offset[0]:.4f} {joint.end_offset[1]:.4f} {joint.end_offset[2]:.4f}")
        lines.append(f"{indent}  }}")
    lines.append(f"{indent}}}")


def animate(joint_order, num_frames):
    """Returns a list (per frame) of dicts {joint_name: [channel values]}."""
    frames = []
    walk_speed = 2.2       # units/frame forward translation
    bob_amp = 1.5           # vertical bob amplitude
    swing_amp = 22.0        # arm/leg swing amplitude, degrees
    sway_amp = 4.0          # spine/head sway amplitude, degrees

    for f in range(num_frames):
        t = f / num_frames
        phase = 2.0 * math.pi * t * 2.0  # 2 full swing cycles over the clip
        swing = math.sin(phase)
        vals = {}

        vals["Hips"] = [
            0.0, 90.0 + bob_amp * abs(math.sin(phase)), walk_speed * f,
            0.0, 0.0, 0.0,
        ]
        vals["Spine"] = [sway_amp * 0.3 * math.sin(phase), 0.0, 0.0]
        vals["Spine1"] = [sway_amp * 0.3 * math.sin(phase), 0.0, 0.0]
        vals["Neck"] = [0.0, 0.0, 0.0]
        vals["Head"] = [sway_amp * 0.4 * math.sin(phase + 0.3), 0.0, 0.0]

        # Arms swing opposite to same-side leg (natural walk cycle).
        #
        # NOTE on channel choice: empirically (verified against this exact
        # BVH channel order -- Zrotation Xrotation Yrotation -- as
        # interpreted by Blender 5.1's BVH importer), the *first* (Z)
        # component bends a bone visibly in the world front-back plane
        # regardless of the bone's rest orientation, while the *second*
        # (X) component is a no-op twist around the bone's own length for
        # every bone tested here. So all "swing" motion below is authored
        # on index 0, not (as would be the naive guess) index 1. Index 2
        # (Y) is used only for the forearm's elbow-ish flex, which bends
        # in a different (vertical) plane -- acceptable for a stylized
        # test rig, not anatomically exact.
        vals["LeftShoulder"] = [0.0, 0.0, 0.0]
        vals["LeftArm"] = [swing_amp * swing, 0.0, 0.0]
        vals["LeftForeArm"] = [0.0, 0.0, 10.0 + 8.0 * max(0.0, swing)]
        vals["LeftHand"] = [0.0, 0.0, 0.0]

        vals["RightShoulder"] = [0.0, 0.0, 0.0]
        vals["RightArm"] = [-swing_amp * swing, 0.0, 0.0]
        vals["RightForeArm"] = [0.0, 0.0, 10.0 + 8.0 * max(0.0, -swing)]
        vals["RightHand"] = [0.0, 0.0, 0.0]

        # Legs swing opposite to arms on the same side (i.e. opposite to
        # each other), classic contralateral walk pattern. See the channel
        # note above the arm block -- stride swing goes on index 0 (Z),
        # not index 1 (X), which is a no-op twist for these bones.
        vals["LeftUpLeg"] = [-swing_amp * 0.8 * swing, 0.0, 0.0]
        vals["LeftLeg"] = [15.0 + 15.0 * max(0.0, swing), 0.0, 0.0]
        vals["LeftFoot"] = [-5.0 * swing, 0.0, 0.0]
        vals["LeftToeBase"] = [0.0, 0.0, 0.0]

        vals["RightUpLeg"] = [swing_amp * 0.8 * swing, 0.0, 0.0]
        vals["RightLeg"] = [15.0 + 15.0 * max(0.0, -swing), 0.0, 0.0]
        vals["RightFoot"] = [5.0 * swing, 0.0, 0.0]
        vals["RightToeBase"] = [0.0, 0.0, 0.0]

        frames.append(vals)
    return frames


def generate_bvh_text():
    hips = build_skeleton()
    order = flatten_order(hips, [])

    lines = ["HIERARCHY"]
    write_hierarchy(hips, 0, lines)

    frames = animate(order, NUM_FRAMES)

    lines.append("MOTION")
    lines.append(f"Frames: {NUM_FRAMES}")
    lines.append(f"Frame Time: {FRAME_TIME:.6f}")
    for frame_vals in frames:
        row = []
        for j in order:
            row.extend(frame_vals[j.name])
        lines.append(" ".join(f"{v:.4f}" for v in row))

    return "\n".join(lines) + "\n"


def main():
    out_path = sys.argv[1] if len(sys.argv) > 1 else "AIPipeline/temp/test_walk.bvh"
    os.makedirs(os.path.dirname(os.path.abspath(out_path)), exist_ok=True)
    text = generate_bvh_text()
    with open(out_path, "w") as f:
        f.write(text)
    n_joints = text.count("OFFSET")
    print(f"[make_test_bvh] Wrote {out_path} ({n_joints} joints, {NUM_FRAMES} frames @ {FPS}fps)")
    return out_path


if __name__ == "__main__":
    main()
