using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform MaxYPosition;
    public Transform MinYPosition;
    public Transform ChaseTarget;

    // Update is called once per frame
    void Update()
    {
        if (ChaseTarget == null) return;
        transform.position = Vector3.Lerp(transform.position, ChaseTarget.position, Time.deltaTime * 5) + Vector3.back * 10;
        if (transform.position.y > MaxYPosition.position.y)
            transform.position = new Vector3(transform.position.x, MaxYPosition.position.y, transform.position.z);
        if (transform.position.y < MinYPosition.position.y)
            transform.position = new Vector3(transform.position.x, MinYPosition.position.y, transform.position.z);
    }
}
