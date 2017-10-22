using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
	private static ScoreManager instance;
	public static ScoreManager Instance
	{
		get
		{
			return instance;
		}
	}

	public Text scoreText;
	public int score;

	void Awake()
	{
		instance = this;
		score = 0;
	}
	
	void Update ()
	{
		scoreText.text = score.ToString();
	}
}
