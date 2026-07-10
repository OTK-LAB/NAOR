# Phase 5.3 Plan: MoMask (Text-to-Motion) Setup

## Goal
Install the MoMask repository, set up its Apple Silicon (MPS) compatible PyTorch environment, and download the necessary pre-trained motion tensors.

## 1. Clone Repository
- **Target Dir**: `/Volumes/aebasol_1tb/Ob/AI_Tools/MoMask`
- Command: `git clone https://github.com/EricGuo5513/momask-codes.git /Volumes/aebasol_1tb/Ob/AI_Tools/MoMask`

## 2. Python Environment & PyTorch (MPS)
- Navigate to `/Volumes/aebasol_1tb/Ob/AI_Tools/MoMask`.
- Create a virtual environment: `python3 -m venv venv`
- Activate and install MPS-compatible PyTorch: 
  `./venv/bin/pip install torch torchvision torchaudio`
- Install standard Text-to-Motion dependencies (e.g., transformers, clip, librosa, scipy):
  `./venv/bin/pip install -r requirements.txt` (or install manually if requirements.txt is missing).

## 3. Model Weights Download
- Create a `checkpoints/t2m/` directory inside MoMask.
- Download the pre-trained Text-to-Motion weights (VQ-VAE and Transformer checkpoints) into this folder. We will use a bash script to fetch them from the official HuggingFace mirrors (`https://huggingface.co/spaces/EricG/MoMask/resolve/main/checkpoints/...`) to avoid Google Drive CLI limits.
