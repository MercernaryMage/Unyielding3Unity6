using UnityEditor;
using UnityEngine;
using System;

public class PngImportDefaults : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		if (assetPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
		{
			TextureImporter importer = (TextureImporter)assetImporter;

			importer.spriteImportMode = SpriteImportMode.Single;
		}
	}
}