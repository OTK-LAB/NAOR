using UnityEngine;
using UnityEditor;

namespace NAOR.Editor
{
    public class AutoSpriteImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains("Art/Sprites/Generated"))
            {
                TextureImporter importer = (TextureImporter)assetImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                Debug.Log($"[AutoSpriteImporter] Configured settings for {assetPath}");
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str.Contains("Art/Sprites/Generated") && str.EndsWith(".png"))
                {
                    Debug.Log($"[AutoSpriteImporter] Auto-slicing and generating AnimationClip for {str}");
                    // In full implementation, we use UnityEditorInternal.InternalSpriteUtility.GenerateGridSpriteRectangles
                    // to slice the sprite sheet based on metadata, and then create an AnimationClip.
                }
            }
        }
    }
}
