using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class CustomAssetUtil
{
    public static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        var newPath = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(asset, newPath);
        AssetDatabase.SaveAssets();

        return asset;
    }
}
#endif