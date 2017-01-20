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
    public float DashSpeed = 700f;
    public Wave WavePrefab;
    public float MaxChaseDuration = 3f;
    public float MinChaseDuration = 2f;
    public float WaveCooldown = .5f;
    public float DashTime = 1f;
    public GameObject TrailPrefab;


    private Rigidbody2D _rigidbody;
    private Vector2 _movementVector;
    private float _chaseDuration;
    private bool _buttonDown;
    private float _currentWaveCooldown;
    private float _currentDashCooldown;
    private WaveMode _currentWaveMode = WaveMode.Follow;
    [SerializeField]
    private bool _isDashing;
    // Use this for initialization
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _buttonDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (TrailPrefab != null && _isDashing)
            SpawnTrail();
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

    private void SpawnTrail()
    {
        var trail = Instantiate(TrailPrefab, transform.position, transform.rotation);
        trail.transform.localScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (_isDashing)
            _rigidbody.AddForceAtPosition(_movementVector * DashSpeed, transform.position);
        else
            _rigidbody.AddForceAtPosition(_movementVector * Speed, transform.position);
    }

    private void RotationUpdate()
    {
        transform.localScale = _movementVector.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);

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
        if (_currentDashCooldown >= 0)
        {
            _currentDashCooldown -= Time.deltaTime;
        }
        else
        {
            _currentDashCooldown = 0;
        }
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
            StartCoroutine(SetAttackMode());
        }
        if (_buttonDown && Math.Abs(_currentWaveCooldown) < float.Epsilon)
        {
            var wave = Instantiate(WavePrefab, transform.position, Quaternion.identity);
            wave.Mode = _currentWaveMode;
            if (_currentWaveMode == WaveMode.Attack)
            {
                Dash();
            }
            _currentWaveCooldown = WaveCooldown;
        }
    }

    private IEnumerator SetAttackMode()
    {
        _currentWaveMode = WaveMode.Attack;
        yield return new WaitForSeconds(WaveCooldown);
        _currentWaveMode = WaveMode.Follow;
    }

    private void ControlMovement()
    {
        if (!_isDashing)
            _movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }

    public void SetMovementVector(Vector2 source)
    {
        if (Mode != WhaleMode.Follower || _isDashing) return;
        _movementVector = (source - (Vector2)transform.position).normalized;
        _chaseDuration = Random.Range(MinChaseDuration, MaxChaseDuration);
    }

    public void Dash()
    {
        if (_currentDashCooldown > 0) return;

        StartCoroutine(SetDashState());
    }

    private IEnumerator SetDashState()
    {
        _isDashing = true;
        yield return new WaitForSeconds(DashTime);
        _isDashing = false;
        _movementVector = _movementVector * -1;
    }
}
public enum WhaleMode { Alpha, Follower }