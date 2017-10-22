using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossChecker : MonoBehaviour
{
	public Transform target;
	public float maxOffset;

	void Update()
	{
		if (transform.position.y - target.transform.position.y < -maxOffset / 2.0f)
		{
			transform.position = new Vector3(0.0f, target.transform.position.y + target.transform.position.y % maxOffset, transform.position.z);
		}
	}

}
