# Phase 5.1 Plan: Install Core Applications

## Goal
Establish the base AI tools (Blender and ComfyUI) directly on the external SSD so that generation occurs entirely locally without cluttering the native Mac SSD.

## 1. Install Blender
- Execute `brew install --cask blender` via terminal to install the official Blender application.
- This will provide the `blender` executable, which we will use in headless mode (`-b`) during the orchestration.

## 2. Setup AI Tools Directory
- Create a dedicated folder on the 1TB SSD for AI applications to keep the Unity project root clean:
  `mkdir -p /Volumes/aebasol_1tb/Ob/AI_Tools`

## 3. Install ComfyUI
- Navigate to the new tools directory.
- Clone the ComfyUI repository:
  `git clone https://github.com/comfyanonymous/ComfyUI.git`
- Navigate into the `ComfyUI` folder.

## 4. Setup Python Environment for ComfyUI
- Create a dedicated virtual environment inside ComfyUI:
  `python3 -m venv venv`
- Activate the environment and install PyTorch with Apple Silicon (MPS) support:
  `./venv/bin/pip install torch torchvision torchaudio`
- Install ComfyUI's dependencies:
  `./venv/bin/pip install -r requirements.txt`
