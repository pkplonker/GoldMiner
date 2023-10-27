using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class KillPlayerCollider : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Rigidbody>().isKinematic = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerMovement>())
		{
			other.GetComponent<PlayerStats>();
			Debug.Log("Kill player");
		}
	}
}