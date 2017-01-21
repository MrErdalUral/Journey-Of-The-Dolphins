using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinAttack : MonoBehaviour
{
    public float Damage = 20;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Enemy" || !GetComponentInParent<Dolphin>().IsDashing) return;
        
        var shark = collision.GetComponent<Shark>();
        if (shark != null)
        {
            shark.Health -= Damage;
        }
    }
}
