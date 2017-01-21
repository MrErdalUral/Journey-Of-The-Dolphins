using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Shark : MonoBehaviour
{
    public float Speed = 5f;
    public float DashSpeed = 10f;
    public float Health = 100f;
    public float Damage = 25f;
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private Vector3 _pos1;
    [SerializeField]
    private Vector3 _pos2;
    private LayerMask _mask;
    private Vector3 _currentDest;
    [SerializeField]
    private float _currentSpeed;
    // Use this for initialization
    void Start()
    {
        _currentSpeed = Speed;
        _mask = LayerMask.GetMask("Dolphin", "FollowerDolphin");
        _rigidbody = GetComponent<Rigidbody2D>();
        _currentDest = new Vector3(Random.Range(_pos1.x, _pos2.x), Random.Range(_pos1.y, _pos2.y), 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentDest, Time.deltaTime*_currentSpeed);
        transform.localScale = new Vector3(Mathf.Sign(_currentDest.x - transform.position.x), 1, 1);
        if ((transform.position - _currentDest).sqrMagnitude < float.Epsilon)
        {
            _currentSpeed = Speed;

            _currentDest = new Vector3(Random.Range(_pos1.x, _pos2.x), Random.Range(_pos1.y, _pos2.y), 0);
        }

        RaycastHit2D hit;
        hit =
            Physics2D.CircleCast(
                transform.position + new Vector3(Mathf.Sign(_currentDest.x - transform.position.x), 0, 0)*70, 30f,
                new Vector2(Mathf.Sign(_currentDest.x - transform.position.x), 0), 300f, _mask);
        if (hit.transform != null)
        {
            _currentDest = hit.transform.position;
            _currentSpeed = DashSpeed;
        }

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var dolphin = collision.GetComponent<Dolphin>();
        if (dolphin == null) return;
        _currentSpeed = Speed;
        _currentDest = new Vector3(Random.Range(_pos1.x, _pos2.x), Random.Range(_pos1.y, _pos2.y), 0);
        dolphin.Health -= Damage;
    }
}
