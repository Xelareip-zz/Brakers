using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class Parameters : ParameterBase
{
	private static string SAVE_FILE = Application.persistentDataPath + "/params.json";

	private static Parameters instance;
	public static Parameters Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new Parameters();
				instance.Load();
			}
			return instance;
		}
	}

	[Parameter("Left input")]
	public INPUT_TYPE leftInput = INPUT_TYPE.BRAKE;
	[Parameter("Right input")]
	public INPUT_TYPE rightInput = INPUT_TYPE.BRAKE;
	[Parameter]
	public BRAKE_TYPE brakeType = BRAKE_TYPE.PROPORTIONNAL;

	[Parameter]
	public float boostDelay = 3;
	[Parameter]
	public float boostStrength = 5;
	
	[Parameter]
	public float minSpeed = 0;
	[Parameter]
	public float maxSpeed = 50;
	[Parameter]
	public float accelerationSpeed = 3;
	[Parameter]
	public float brakeSpeed = 1.5f;

	[Parameter]
	public bool speedScores = true;

	[Parameter]
	public bool platformScores = false;
	[Parameter]
	public int maxPlatformsCount = 6;
	[Parameter]
	public float platformDistance = 5;

	[Parameter("Cam size min")]
	public float minCamSize = 10;
	[Parameter("Cam scale")]
	public float camScale = 1;
	[Parameter("Near miss distance")]
	public float nearMissDistance = 0;
	[Parameter("Platform distance scale")]
	public float platformDistanceScale = 1.0f;

	private Parameters()
	{
	}

	public override void Load()
	{
		if (File.Exists(SAVE_FILE))
		{
			string content = File.ReadAllText(SAVE_FILE);
			JsonUtility.FromJsonOverwrite(content, this);
		}
	}

	public override void Save()
	{
		File.WriteAllText(SAVE_FILE, JsonUtility.ToJson(this));
	}

	public override void Reset()
	{
		File.Delete(SAVE_FILE);
	}

	public override void DeleteInstance()
	{
		instance = null;
	}
}
