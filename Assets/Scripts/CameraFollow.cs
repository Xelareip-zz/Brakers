using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float offset;
	public float speed;

	public bool canMove = true;

	void Update()
	{
		if (target != null && canMove)
		{
			transform.position += (new Vector3(transform.position.x, target.transform.position.y + offset, transform.position.z) - transform.position) * speed * Time.deltaTime;
		}
	}
}
