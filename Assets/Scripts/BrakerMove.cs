using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakerMove : MonoBehaviour
{
	public bool goesLeft;
	public Vector2 bounds;
	public float speed;

	void Update ()
	{
		Vector3 targetPoint = new Vector3(goesLeft ? bounds.x : bounds.y, transform.position.y, transform.position.z);
		Vector3 moveToTarget = targetPoint - transform.position;
        Vector3 move = moveToTarget.normalized * speed * Time.deltaTime;

		if (move.magnitude == 0.0f || move.magnitude >= moveToTarget.magnitude)
		{
			move = Vector3.ClampMagnitude(move, moveToTarget.magnitude);
			goesLeft = !goesLeft;
		}


		transform.position += move;
	}
}
