using UnityEditor;
using UnityEngine;
using System;

#if UNITY_EDITOR
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
#endif