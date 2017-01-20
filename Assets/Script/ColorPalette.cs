using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ColorPalette : ScriptableObject
{
#if UNITY_EDITOR

    [MenuItem("Assets/Color Palette", false, 1)]
    public static void CreateColorPalette()
    {
        if (Selection.activeObject is Texture2D)
        {
            // ReSharper disable once TryCastAlwaysSucceeds
            var selectedTexture = Selection.activeObject as Texture2D;
            var selectionPath = AssetDatabase.GetAssetPath(selectedTexture)
                .Replace(".png", "-color-palette.asset");
            var newPalette = CustomAssetUtil.CreateAsset<ColorPalette>(selectionPath);
            newPalette.Source = selectedTexture;
            newPalette.ResetPalette();

            Debug.Log("Creating a palette! " + selectionPath);
        }
        else
        {
            Debug.Log("Selection is not a Texture 2D");
        }
    }
    [MenuItem("Assets/Color Palette", true)]
    public static bool ValidateSelection()
    {
        return Selection.activeObject is Texture2D;
    }
#endif
    public Texture2D Source;
    public List<Color> Palette = new List<Color>();
    public List<Color> NewPalette = new List<Color>();
    public Texture2D CachedTexture;
    public MaterialPropertyBlock CachedBlock;

    private List<Color> BuildPalette(Texture2D texture)
    {
        var palette = new List<Color>();
        var colors = texture.GetPixels();
        foreach (var color in colors)
        {
            if (palette.Contains(color) || Math.Abs(color.a - 1) > float.Epsilon) continue;

            palette.Add(color);
        }
        return palette;
    }

    public void ResetPalette()
    {
        Palette = BuildPalette(Source);
        NewPalette = new List<Color>(Palette);
    }

    public Color GetColor(Color color)
    {
        for (int i = 0; i < Palette.Count; i++)
        {
            if (Palette[i] == color)
                return NewPalette[i];
        }
        return color;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ColorPalette))]
public class ColorPaletteEditor : Editor
{
    public ColorPalette ColorPalette;

    void OnEnable()
    {
        ColorPalette = target as ColorPalette;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Source Texture");
        ColorPalette.Source = EditorGUILayout.ObjectField(ColorPalette.Source, typeof(Texture2D), false) as Texture2D;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Current Color");
        GUILayout.Label("New Color");
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < ColorPalette.Palette.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ColorField(ColorPalette.Palette[i]);
            ColorPalette.NewPalette[i] = EditorGUILayout.ColorField(ColorPalette.NewPalette[i]);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Reset Palette"))
        {
            ColorPalette.ResetPalette();
        }
        if (GUILayout.Button("Apply Changes"))
            ColorPalette.CachedTexture = null;
        EditorUtility.SetDirty(ColorPalette); // saves the data inside the scriptable object when play mod is closed
        //base.OnInspectorGUI();
    }
}
#endif
