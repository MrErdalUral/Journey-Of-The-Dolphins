using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class PaletteSwapper : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public ColorPalette[] Palettes;

    [Range(0, 50)]
    public int CurrentPaletteIndex = 0;

    private Texture2D _texture;
    //private MaterialPropertyBlock _block;
    // Use this for initialization
    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (Palettes.Length > CurrentPaletteIndex)
            SwapColors(Palettes[CurrentPaletteIndex]);
    }

    void SwapColors(ColorPalette palette)
    {
        if (palette.CachedTexture == null)//if cachet texture is not null we do not need to recreate the texture since it will be the same texture that gets created
        {
            InitPaletteTexture(palette);
        }

    }

    private void InitPaletteTexture(ColorPalette palette)
    {
        _texture = SpriteRenderer.sprite.texture;
        var w = _texture.width;
        var h = _texture.height;

        //clone the texture
        palette.CachedTexture = new Texture2D(w, h);
        palette.CachedTexture.wrapMode = TextureWrapMode.Clamp;
        palette.CachedTexture.filterMode = FilterMode.Point;

        var colors = _texture.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = palette.GetColor(colors[i]);
        }
        // ReSharper disable once PossibleNullReferenceException
        palette.CachedTexture.SetPixels(colors);
        palette.CachedTexture.Apply();// Invoke this method to save the setpixel changes

        palette.CachedBlock = new MaterialPropertyBlock();// This will be applied to the cloned texture in each draw. Used for setting renderer properties
        palette.CachedBlock.SetTexture("_MainTex", palette.CachedTexture); //Name of the Texture will be _MainTex
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (Palettes.Length > CurrentPaletteIndex)
        {
            SwapColors(Palettes[CurrentPaletteIndex]);
            SpriteRenderer.SetPropertyBlock(Palettes[CurrentPaletteIndex].CachedBlock);
        }
        else
        {
            CurrentPaletteIndex = Palettes.Length - 1;
        }
    }
}
