using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dolphin : MonoBehaviour
{
    [SerializeField]
    public WhaleMode Mode = WhaleMode.Alpha;
    public float Speed = 400f;
    public Wave WavePrefab;
    public float MaxChaseDuration = 3f;
    public float MinChaseDuration = 2f;
    public float WaveCooldown = .5f;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementVector;
    private float _chaseDuration;
    private bool _buttonDown;
    private float _currentWaveCooldown = 0;
    // Use this for initialization
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _buttonDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Mode)
        {
            case WhaleMode.Alpha:
                AlphaUpdate();
                break;
            case WhaleMode.Follower:
                FollowerUpdate();
                break;
        }
        RotationUpdate();
    }

    void FixedUpdate()
    {
        _rigidbody.AddForceAtPosition(_movementVector * Speed, transform.position);
    }

    private void RotationUpdate()
    {
        if (_movementVector.x < 0)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        var angle = Mathf.Atan2(_movementVector.y, _movementVector.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FollowerUpdate()
    {
        if (_chaseDuration <= 0)
        {
            _movementVector = Vector2.zero;
            _chaseDuration = 0;
        }
        else
        {
            _chaseDuration -= Time.deltaTime;
        }
    }

    private void AlphaUpdate()
    {
        ControlMovement();
        ControlActions();
    }

    private void ControlActions()
    {
        if (WavePrefab == null) return;

        if (_currentWaveCooldown >= 0)
        {
            _currentWaveCooldown -= Time.deltaTime;
        }
        else
        {
            _currentWaveCooldown = 0;
        }
        if (!_buttonDown && Input.GetButtonDown("Action"))
            _buttonDown = true;
        if (_buttonDown && Input.GetButtonUp("Action"))
        {
            _buttonDown = false;
            _currentWaveCooldown = 0;
        }
        if (_buttonDown && Math.Abs(_currentWaveCooldown) < float.Epsilon)
        {
            Instantiate(WavePrefab, transform.position, Quaternion.identity);
            _currentWaveCooldown = WaveCooldown;
        }
    }

    private void ControlMovement()
    {
        _movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }

    public void SetMovementVector(Vector2 source)
    {
        if (Mode != WhaleMode.Follower) return;
        _movementVector = (source - (Vector2)transform.position).normalized;
        _chaseDuration = Random.Range(MinChaseDuration, MaxChaseDuration);
    }
}
public enum WhaleMode { Alpha, Follower }