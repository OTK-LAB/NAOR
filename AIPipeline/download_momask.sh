#!/bin/bash
cd /Volumes/aebasol_1tb/Ob/AI_Tools/MoMask
source venv/bin/activate

mkdir -p checkpoints/t2m
cd checkpoints/t2m
echo "Downloading HumanML3D models..."
gdown "https://drive.google.com/file/d/1vXS7SHJBgWPt59wupQ5UUzhFObrnGkQ0/view?usp=sharing" -O humanml3d_models.zip
unzip -q humanml3d_models.zip
rm humanml3d_models.zip

cd ..
mkdir -p kit
cd kit
echo "Downloading KIT-ML models..."
gdown "https://drive.google.com/file/d/1FapdHNkxPouasVM8MWgg1f6sd_4Lua2q/view?usp=sharing" -O kit_models.zip
unzip -q kit_models.zip
rm kit_models.zip

echo "MoMask Checkpoints Downloaded."
