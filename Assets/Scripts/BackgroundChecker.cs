using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallCreator : MonoBehaviour
{
	public bool keepRunning;
	public bool execute;

	public float entryCutAngle;
	public float exitCutAngle;
	public float width;

	public MeshFilter filter;
	public PolygonCollider2D polygonCollider;
	public Mesh mesh;
	public List<Vector3> controlPoints = new List<Vector3>();

	public List<Vector3> vertices = new List<Vector3>();
	public List<Vector3> normals = new List<Vector3>();
	public List<Vector2> uvs = new List<Vector2>();
	public List<int> triangles = new List<int>();
	public List<Vector2> polygon = new List<Vector2>();

	void Awake ()
	{
		if (Application.isPlaying)
		{
			mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.name = gameObject.GetInstanceID().ToString();
			filter.mesh = mesh;
		}
		UpdateMesh();
	}

#if UNITY_EDITOR
	void Update()
	{
		if (keepRunning)
		{
			execute = false;
			UpdateMesh();
		}
		else if (execute)
		{
			execute = false;
			UpdateMesh();
		}
	}
#endif

	public void UpdateMesh()
	{
		if (controlPoints.Count < 2)
		{
			return;
		}
		vertices.Clear();
		normals.Clear();
		uvs.Clear();
		triangles.Clear();
		polygon.Clear();

		List<Vector3> tempPoints = new List<Vector3>();

		Vector3 mirror = Quaternion.AngleAxis(entryCutAngle, Vector3.forward) * Vector3.up;
		Vector3 initialImage = controlPoints[1] - controlPoints[0];

		Vector3 firstPoint = Vector3.Dot(mirror, initialImage) * 2.0f * mirror - initialImage + controlPoints[0];
		tempPoints.Add(firstPoint);
		tempPoints.AddRange(controlPoints);


		mirror = Quaternion.AngleAxis(exitCutAngle, Vector3.forward) * Vector3.up;
		initialImage = controlPoints[controlPoints.Count - 2] - controlPoints[controlPoints.Count - 1];

		Vector3 lastPoint = Vector3.Dot(mirror, initialImage) * 2.0f * mirror - initialImage + controlPoints[controlPoints.Count - 1];
		tempPoints.Add(lastPoint);

		for (int pointIdx = 1; pointIdx < tempPoints.Count - 1; ++pointIdx)
		{
			Vector3 previousPoint = tempPoints[pointIdx+ - 1];
			Vector3 nextPoint = tempPoints[pointIdx + 1];
			Vector3 point = tempPoints[pointIdx];

			float dot = Mathf.Abs(Vector3.Dot((nextPoint - point).normalized, (previousPoint - point).normalized));
            if (pointIdx > 1 && pointIdx < tempPoints.Count - 2 && dot == 1.0f)
			{
				continue;
			}

			Vector3 bissect = ((previousPoint - point).normalized + (nextPoint - point).normalized).normalized;
			if (bissect == Vector3.zero)
			{
				bissect = new Vector3((previousPoint - point).y, -(previousPoint - point).x, 0.0f).normalized;
            }

			Vector3 intersectLeft = (new Vector3(point.y - previousPoint.y, previousPoint.x - point.x, point.z)).normalized / width;
			Vector3 intersectRight = (new Vector3(previousPoint.y - point.y, point.x - previousPoint.x, point.z)).normalized / width;


			vertices.Add(point + bissect / Vector3.Dot(bissect, intersectLeft));
			vertices.Add(point + bissect / Vector3.Dot(bissect, intersectRight));

			polygon.Add(vertices[vertices.Count - 2]);
			polygon.Insert(0, vertices[vertices.Count - 1]);
		}
		/*
		point = controlPoints[controlPoints.Count - 1];
        vertices.Add(controlPoints[controlPoints.Count - 1] + new Vector3(offsetLeft.x, offsetLeft.y) - Vector3.forward * point.z);
		vertices.Add(point + new Vector3(offsetRight.x, offsetRight.y) - Vector3.forward * point.z);
		normals.Add(Vector3.back);
		normals.Add(Vector3.back);
		uvs.Add(Vector2.zero);
		uvs.Add(Vector2.one);
		polygon.Add(new Vector2(point.x, point.y) + offsetLeft);
		polygon.Insert(0, new Vector2(point.x, point.y) + offsetRight);*/

		for (int triangleIdx = 0; triangleIdx < vertices.Count - 2; ++triangleIdx)
		{
			triangles.Add(triangleIdx);
			triangles.Add(triangleIdx + 1 + triangleIdx % 2);
			triangles.Add(triangleIdx + 2 - triangleIdx % 2);
		}

		for (int i = 0; i < vertices.Count; ++i)
		{
			normals.Add(Vector3.back);
			uvs.Add(Vector2.zero);
		}

		if (Application.isPlaying)
		{
			// Avoids inconsistencies between triangles and vertices
			if (mesh.vertexCount != 0)
			{
				mesh.SetTriangles(new int[3] { 0, 0, 0 }, 0);
			}
			mesh.SetVertices(vertices);
			mesh.SetTriangles(triangles.ToArray(), 0);
			mesh.SetNormals(normals);
			mesh.SetUVs(0, uvs);
			mesh.RecalculateBounds();
			filter.mesh = mesh;
		}
		polygonCollider.points = polygon.ToArray();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.black;
		foreach (Vector3 point in controlPoints)
		{
			Gizmos.DrawSphere(transform.position + point, 0.25f);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		for (int pointIdx = 0; pointIdx < polygonCollider.points.Length; ++pointIdx)
		{
			Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y) + polygonCollider.points[pointIdx], new Vector2(transform.position.x, transform.position.y) + polygonCollider.points[(pointIdx + 1) % polygonCollider.points.Length]);
		}
	}
}
