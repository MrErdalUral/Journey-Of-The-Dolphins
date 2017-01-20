using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public float MaxRadius = 10f;
    public float Speed = 5f;
    public float FadeSpeed = 10f;
    public float Duration = 1f;

    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, Duration);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var dolphinScript = collision.GetComponent<Dolphin>();
        if (dolphinScript == null) return;

        dolphinScript.SetMovementVector((Vector2)transform.position);
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, MaxRadius * transform.localScale, Time.deltaTime * Speed);
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, Mathf.Lerp(_spriteRenderer.color.a, 0, Time.deltaTime * FadeSpeed));
    }
}
