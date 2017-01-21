using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public float MaxRadius = 10f;
    public float Speed = 5f;
    public float FadeSpeed = 10f;
    public float Duration = 1f;
    public WaveMode Mode;
    public Vector2 OriginalWave;
    private PaletteSwapper _paletteSwapper;

    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _paletteSwapper = GetComponent<PaletteSwapper>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, Duration);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var dolphinScript = collision.GetComponent<Dolphin>();
        if (dolphinScript == null) return;
        if (dolphinScript.Mode == DolphinMode.Follower && gameObject.tag != "SmallWave" && gameObject != collision.gameObject)
        {
            var wave = dolphinScript.SendWave();
            wave.OriginalWave = transform.position;
        }
        WaveAction(dolphinScript,this);
    }

    private void WaveAction(Dolphin dolphinScript,Wave waveScript)
    {
        if (waveScript.OriginalWave == Vector2.zero)
            dolphinScript.SetMovementVector(waveScript.transform.position);
        else
        {
            dolphinScript.SetMovementVector(waveScript.OriginalWave);
        }
        if (waveScript.Mode == WaveMode.Attack)
            dolphinScript.Dash();
    }

    void Update()
    {
        _paletteSwapper.CurrentPaletteIndex = (int)Mode;
        transform.localScale = Vector3.Lerp(transform.localScale, MaxRadius * transform.localScale, Time.deltaTime * Speed);
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, Mathf.Lerp(_spriteRenderer.color.a, 0, Time.deltaTime * FadeSpeed));
    }
}
public enum WaveMode { Follow, Attack }
