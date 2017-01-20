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

    private Rigidbody2D _rigidbody;
    private Vector2 _movementVector;
    private float _chaseDuration;
    // Use this for initialization
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
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
        _rigidbody.AddForceAtPosition(_movementVector * Speed, transform.position);
        RotationUpdate();
    }

    private void RotationUpdate()
    {
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
        if (Input.GetButtonDown("Action"))
            Instantiate(WavePrefab, transform.position, Quaternion.identity);
    }

    private void ControlMovement()
    {
        _movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") / 2).normalized;
        if (_movementVector.y >= .5f)
            _movementVector.y = .5f;
    }

    public void SetMovementVector(Vector2 source)
    {
        if (Mode != WhaleMode.Follower) return;
        _movementVector = (source - (Vector2)transform.position).normalized;
        if (_movementVector.y >= .5f)
            _movementVector.y = .5f;
        _chaseDuration = Random.Range(MinChaseDuration, MaxChaseDuration);
    }
}
public enum WhaleMode { Alpha, Follower }