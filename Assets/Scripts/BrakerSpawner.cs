using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakerSpawner : MonoBehaviour
{
	public GameObject platformModel;
	public GameObject player;
	public List<GameObject> platforms = new List<GameObject>();
	public int maxCount;
	public float distance;
	public Vector2 boundaries;

	public bool canScore;

	void Start()
	{
		canScore = Parameters.Instance.platformScores;

		maxCount = Parameters.Instance.maxPlatformsCount;
		distance = Parameters.Instance.platformDistance;
	}

	void Update()
	{
		if (player == null)
		{
			return;
		}
		for (int idx = 0; idx < platforms.Count; ++idx)
		{
			if (platforms[idx] == null)
			{
				platforms.RemoveAt(idx);
				--idx;
			}
			else if (platforms[idx].transform.position.y < player.transform.position.y - 3)
			{
				if (canScore)
				{
					++ScoreManager.Instance.score;
				}
				Destroy(platforms[idx]);
				platforms.RemoveAt(idx);
				--idx;
			}
		}

		while (platforms.Count < maxCount)
		{
			GameObject newPlat = null;
			if (platforms.Count != 0)
			{
				newPlat = Instantiate<GameObject>(platformModel, new Vector3(Random.Range(boundaries.x, boundaries.y), platforms[platforms.Count - 1].transform.position.y + distance, player.transform.position.z), Quaternion.identity);
			}
			else
			{
				newPlat = Instantiate<GameObject>(platformModel, new Vector3(Random.Range(boundaries.x, boundaries.y), player.transform.position.y + distance, player.transform.position.z), Quaternion.identity);
			}
			platforms.Add(newPlat);
		}
	}
}
