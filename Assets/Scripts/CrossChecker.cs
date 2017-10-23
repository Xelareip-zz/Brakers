using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossChecker : MonoBehaviour
{
	public float checkerSize;
	public Material visualMaterial;
	public Transform visualObject;
	public float maxOffset;

	void Update()
	{
		while (visualObject.transform.position.y - Camera.main.transform.position.y < -maxOffset)
		{
			visualObject.transform.position += Vector3.up * maxOffset;
		}

		float orthoSize = (Camera.main.orthographicSize + maxOffset) * 2f;

		visualObject.transform.localScale = new Vector3(orthoSize * Camera.main.aspect, orthoSize, 1);
		orthoSize /= 4.0f;
		visualMaterial.SetFloat("repeatsX", orthoSize * Camera.main.aspect	 / checkerSize);
		visualMaterial.SetFloat("repeatsY", orthoSize						 / checkerSize);
	}

}
