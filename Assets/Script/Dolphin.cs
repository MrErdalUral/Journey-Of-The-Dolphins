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
    public DolphinMode Mode = DolphinMode.Alpha;
    public float Speed = 400f;
    public float DashSpeed = 700f;
    public Wave WavePrefab;
    public float MaxChaseDuration = 3f;
    public float MinChaseDuration = 2f;
    public float WaveCooldown = .5f;
    public float DashTime = 1f;
    public GameObject TrailPrefab;
    public float Health = 100f;


    private Rigidbody2D _rigidbody;
    private Vector2 _movementVector;
    private float _chaseDuration;
    private bool _buttonDown;
    private float _currentWaveCooldown;
    private float _currentDashCooldown;
    private WaveMode _currentWaveMode = WaveMode.Follow;
    [SerializeField]
    private bool _isJumping;
    private float _timeCounter;
    public bool IsDashing;

    public Vector2 MovementVector
    {
        get { return _movementVector; }
        set { _movementVector = value; }
    }

    // Use this for initialization
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _buttonDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        _timeCounter += Time.deltaTime;
        if (TrailPrefab != null && IsDashing && _timeCounter >= .1f)
        {
            SpawnTrail();
            _timeCounter = 0;
        }
        switch (Mode)
        {
            case DolphinMode.Alpha:
                AlphaUpdate();
                break;
            case DolphinMode.Follower:
                FollowerUpdate();
                break;
        }
        RotationUpdate();


        if (_isJumping)
        {
            _rigidbody.AddForce(Vector2.down * 800, ForceMode2D.Force);
        }
        if (IsDashing)
            _rigidbody.AddForce(MovementVector * DashSpeed, ForceMode2D.Force);
        else
            _rigidbody.AddForce(MovementVector * Speed, ForceMode2D.Force);

        ControlHealth();
    }

    private void ControlHealth()
    {
        if (Health <= 0)
        {
            if (Mode == DolphinMode.Alpha)
            {
                var newDolphin = GameObject.FindGameObjectWithTag("FollowerDolphin");
                if (newDolphin != null)
                {
                    var instance = Instantiate(this, newDolphin.transform.position, Quaternion.identity);
                    instance.Health = newDolphin.GetComponent<Dolphin>().Health;
                    Destroy(newDolphin);
                    var camera = Camera.main;
                    camera.GetComponent<CameraController>().ChaseTarget = instance.transform;
                }
            }
            Destroy(gameObject);
        }
        else
        {
            Health += 3 * Time.deltaTime;
            if (Health >= 100)
            {
                Health = 100;
            }
        }
    }

    private void SpawnTrail()
    {
        var trail = Instantiate(TrailPrefab, transform.position, transform.rotation);
        trail.transform.localScale = transform.localScale;
    }


    private void RotationUpdate()
    {
        var lookingVector = _isJumping ? _rigidbody.velocity.normalized : MovementVector;
        transform.localScale = lookingVector.x < 0 ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);

        var angle = Mathf.Atan2(lookingVector.y, lookingVector.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FollowerUpdate()
    {
        if (_chaseDuration <= 0)
        {
            MovementVector = Vector2.zero;
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
            SendWave();
        }
    }

    public Wave SendWave()
    {
        if (WavePrefab == null) return null;
        var wave = Instantiate(WavePrefab, transform.position, Quaternion.identity);
        wave.Mode = _currentWaveMode;
        if (_currentWaveMode == WaveMode.Attack)
        {
            Dash();
        }
        _currentWaveCooldown = WaveCooldown;
        return wave;
    }

    private IEnumerator SetAttackMode()
    {
        _currentWaveMode = WaveMode.Attack;
        yield return new WaitForSeconds(WaveCooldown);
        _currentWaveMode = WaveMode.Follow;
    }

    private void ControlMovement()
    {
        MovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }

    public void SetMovementVector(Vector2 source)
    {
        if (Mode != DolphinMode.Follower || IsDashing) return;
        MovementVector = (source - (Vector2)transform.position).normalized;
        _chaseDuration = Random.Range(MinChaseDuration, MaxChaseDuration);
    }

    public void Dash()
    {
        if (_currentDashCooldown > 0) return;

        StartCoroutine(SetDashState());
    }

    private IEnumerator SetDashState()
    {
        IsDashing = true;
        yield return new WaitForSeconds(DashTime);
        IsDashing = false;
        MovementVector = MovementVector * -1;
    }

    public void JumpEnter()
    {
        _isJumping = true;
    }

    public void JumpExit()
    {
        _isJumping = false;
    }
}
public enum DolphinMode { Alpha, Follower }