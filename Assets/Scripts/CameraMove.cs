using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public Vector3 speed;

	void Update()
	{
		transform.position += speed * Time.deltaTime;
	}
}
