using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftCollider : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D carpan)
	{
		carpan.transform.position += new Vector3 (6075, 0, 0);
	}
}
