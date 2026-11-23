using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundCollection))]
public class SoundCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Get reference to the target script
        SoundCollection soundCollection = (SoundCollection)target;

        if (GUILayout.Button("Update"))
        {
            FilloutData(soundCollection);
        }
    }

    public static void FilloutData(SoundCollection soundCollection)
    {
        soundCollection.sounds.Clear();

        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        Dictionary<string, string> pathsByName = new Dictionary<string, string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
			AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

            if (pathsByName.ContainsKey(clip.name))
            {
                Debug.LogError($"Duplicate sound name \"{clip.name}\": {path} conflicts with {pathsByName[clip.name]}");
            }
            else
            {
                pathsByName.Add(clip.name, path);
            }

            soundCollection.sounds.Add(clip);
        }

        EditorUtility.SetDirty(soundCollection);
        AssetDatabase.SaveAssets();
    }
}
