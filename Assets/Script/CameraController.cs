using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform ChaseTarget;

    // Update is called once per frame
    void Update()
    {
        if (ChaseTarget == null) return;
        transform.position = Vector3.Lerp(transform.position, ChaseTarget.position, Time.deltaTime * 5) + Vector3.back * 10;
    }
}
