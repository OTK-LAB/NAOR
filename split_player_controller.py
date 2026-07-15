import os

filepath = 'Assets/Scripts/Prototype/Player/PlayerController.cs'

with open(filepath, 'r') as f:
    lines = f.readlines()

# 0-indexed line ranges (inclusive)
movement_ranges = [(335, 502), (523, 544)]
combat_ranges = [(503, 521), (616, 780)]

def extract_lines(ranges):
    extracted = []
    for start, end in ranges:
        extracted.extend(lines[start:end+1])
    return "".join(extracted)

movement_code = extract_lines(movement_ranges)
combat_code = extract_lines(combat_ranges)

# Core code: everything NOT in those ranges, PLUS change 'public class' to 'public partial class'
core_lines = []
for i, line in enumerate(lines):
    in_extracted = False
    for start, end in movement_ranges + combat_ranges:
        if start <= i <= end:
            in_extracted = True
            break
    if not in_extracted:
        if "public class PlayerController : MonoBehaviour" in line:
            line = line.replace("public class PlayerController", "public partial class PlayerController")
        core_lines.append(line)

core_code = "".join(core_lines)

movement_file = """using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
""" + movement_code + "}\n"

combat_file = """using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
""" + combat_code + "}\n"

with open('Assets/Scripts/Prototype/Player/PlayerController.Movement.cs', 'w') as f:
    f.write(movement_file)

with open('Assets/Scripts/Prototype/Player/PlayerController.Combat.cs', 'w') as f:
    f.write(combat_file)

with open(filepath, 'w') as f:
    f.write(core_code)

print("Files successfully split!")
