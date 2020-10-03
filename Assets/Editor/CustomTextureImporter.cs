using UnityEditor;
using UnityEngine;

public class CustomTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 16;
    }
}

