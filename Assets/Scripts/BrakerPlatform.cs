using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakerPlatform : MonoBehaviour
{
	private float safeDistance;

	public float initialHeight;

	void Awake()
	{
		safeDistance = Parameters.Instance.nearMissDistance;
		initialHeight = transform.localScale.y;
	}

	void Update ()
	{
		transform.localScale = new Vector3(transform.localScale.x, initialHeight * (Camera.main.orthographicSize / Parameters.Instance.minCamSize), transform.localScale.z);
	}
}
