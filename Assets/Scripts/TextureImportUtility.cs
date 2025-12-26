#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class TextureImportUtility
{
    [MenuItem("Tools/Enable ReadWrite For Selected Textures")]
    static void EnableReadWrite()
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;
            ti.isReadable = true;
            ti.SaveAndReimport();
            Debug.Log("Set readable: " + path);
        }
    }
}
#endif