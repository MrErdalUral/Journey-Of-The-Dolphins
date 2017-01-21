using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var dolphinScript = collision.GetComponent<Dolphin>();
        if (dolphinScript == null) return;
        dolphinScript.JumpEnter();
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        var dolphinScript = collision.GetComponent<Dolphin>();
        if (dolphinScript == null) return;
        dolphinScript.JumpExit();
    }
}
