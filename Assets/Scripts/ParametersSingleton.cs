using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrefabsPaths
{
	public static string INPUT_FIELD_PREFAB_PATH = "ParametersPrefabs/InputField";
	public static string BOOL_FIELD_PREFAB_PATH = "ParametersPrefabs/BoolField";
	public static string ENUM_FIELD_PREFAB_PATH = "ParametersPrefabs/EnumField";
}

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ParameterAttribute : Attribute
{
	private string name;

	public ParameterAttribute()
	{
		name = "";
	}

	public ParameterAttribute(string attributeName)
	{
		name = attributeName;
	}

	public string GetName()
	{
		return name;
	}
}

public abstract class ParameterEditor
{
	private static GameObject _holder;
	public static GameObject holder
	{
		get
		{
			if (_holder == null)
			{
				Canvas canvas = GameObject.FindObjectOfType<Canvas>();
				if (canvas != null)
				{
					_holder = canvas.gameObject;
				}
			}
			return _holder;
		}
		set
		{
			_holder = value;
		}
	}

	public GameObject editor;
	public RectTransform editorTransform
	{
		get
		{
			return editor.transform as RectTransform;
		}
	}

	public string name;

	public ParameterEditor()
	{
		name = "";
	}

	public ParameterEditor(string _name)
	{
		name = _name;
	}

	public abstract object GetValue();
}

public class StringParameterEditor : ParameterEditor
{
	private InputField inputField;

	public StringParameterEditor(string name) : base(name)
	{
		GameObject model = Resources.Load<GameObject>(PrefabsPaths.INPUT_FIELD_PREFAB_PATH);
		editor = GameObject.Instantiate<GameObject>(model, ParameterEditor.holder.transform);
		inputField = editor.GetComponentInChildren<InputField>();
		Text[] children = editor.GetComponentsInChildren<Text>();
		foreach (Text trans in children)
		{
			if (trans.gameObject.name == "Label")
			{
				trans.text = name;
			}
		}
	}

	public StringParameterEditor() : this("")
	{
	}

	public override object GetValue()
	{
		return inputField.text;
	}
}

public class FloatParameterEditor : ParameterEditor
{
	private InputField inputField;
	private float lastValue;

	public FloatParameterEditor(string name, float val) : base(name)
	{
		lastValue = val;
		GameObject model = Resources.Load<GameObject>(PrefabsPaths.INPUT_FIELD_PREFAB_PATH);
		editor = GameObject.Instantiate<GameObject>(model, ParameterEditor.holder.transform);
		inputField = editor.GetComponentInChildren<InputField>();
		inputField.contentType = InputField.ContentType.DecimalNumber;
		inputField.text = lastValue.ToString();
		Text[] children = editor.GetComponentsInChildren<Text>();
		foreach (Text trans in children)
		{
			if (trans.gameObject.name == "Label")
			{
				trans.text = name;
			}
		}
	}

	public FloatParameterEditor() : this("", 0.0f)
	{
	}

	public override object GetValue()
	{
		float val = 0;
		if (float.TryParse(inputField.text, out val))
		{
			lastValue = val;
		}
		return lastValue;
	}
}

public class IntParameterEditor : ParameterEditor
{
	private InputField inputField;
	private int lastValue;

	public IntParameterEditor(string name, int val) : base(name)
	{
		lastValue = val;
		GameObject model = Resources.Load<GameObject>(PrefabsPaths.INPUT_FIELD_PREFAB_PATH);
		editor = GameObject.Instantiate<GameObject>(model, ParameterEditor.holder.transform);
		inputField = editor.GetComponentInChildren<InputField>();
		inputField.contentType = InputField.ContentType.IntegerNumber;
		inputField.text = lastValue.ToString();
		Text[] children = editor.GetComponentsInChildren<Text>();
		foreach (Text trans in children)
		{
			if (trans.gameObject.name == "Label")
			{
				trans.text = name;
			}
		}
	}

	public IntParameterEditor() : this("", 0)
	{
	}

	public override object GetValue()
	{
		int val = 0;
		if (int.TryParse(inputField.text, out val))
		{
			lastValue = val;
		}
		return lastValue;
	}
}

public class BooleanParameterEditor : ParameterEditor
{
	private Toggle inputField;
	private bool lastValue;

	public BooleanParameterEditor(string name, bool val) : base(name)
	{
		lastValue = val;
		GameObject model = Resources.Load<GameObject>(PrefabsPaths.BOOL_FIELD_PREFAB_PATH);
		editor = GameObject.Instantiate<GameObject>(model, ParameterEditor.holder.transform);
		inputField = editor.GetComponentInChildren<Toggle>();
		inputField.isOn = lastValue;
		Text[] children = editor.GetComponentsInChildren<Text>();
		foreach (Text trans in children)
		{
			if (trans.gameObject.name == "Label")
			{
				trans.text = name;
			}
		}
	}

	public BooleanParameterEditor() : this("", false)
	{
	}

	public override object GetValue()
	{
		lastValue = inputField.isOn;
		return lastValue;
	}
}

public class EnumParameterEditor : ParameterEditor
{
	private Type enumType;
	private Dropdown inputField;

	public EnumParameterEditor(string name, Type type, string val) : base(name)
	{
		enumType = type;
		GameObject model = Resources.Load<GameObject>(PrefabsPaths.ENUM_FIELD_PREFAB_PATH);
		editor = GameObject.Instantiate<GameObject>(model, ParameterEditor.holder.transform);
		
		inputField = editor.GetComponentInChildren<Dropdown>();
		List<string> names = new List<string>();
		names.AddRange(Enum.GetNames(enumType));
		inputField.ClearOptions();
		inputField.AddOptions(names);

		for (int idx = 0; idx < names.Count; ++idx)
		{
			if (inputField.options[idx].text == val)
			{
				inputField.value = idx;
				break;
			}
		}

		Text[] children = editor.GetComponentsInChildren<Text>();
		foreach (Text trans in children)
		{
			if (trans.gameObject.name == "Label")
			{
				trans.text = name;
			}
		}
	}

	public override object GetValue()
	{
		return Enum.Parse(enumType, inputField.options[inputField.value].text);
	}
}

[Serializable]
public class Parameters
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
				if (File.Exists(SAVE_FILE))
				{
					string content = File.ReadAllText(SAVE_FILE);
					JsonUtility.FromJsonOverwrite(content, instance);
	            }
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
	public float maxSpeed = float.MaxValue;
	[Parameter]
	public float accelerationSpeed = 3;
	[Parameter]
	public float brakeSpeed = 5;

	[Parameter]
	public bool speedScores = true;

	[Parameter]
	public bool platformScores = false;
	[Parameter]
	public int maxPlatformsCount = 6;
	[Parameter]
	public float platformDistance = 5;

	private Parameters()
	{
	}

	public void Save()
	{
		File.WriteAllText(SAVE_FILE, JsonUtility.ToJson(this));
	}
}

public class ParametersSingleton : MonoBehaviour
{
	public GUIStyle style;

	[NonSerialized]
	public Parameters target;
	public GameObject uiHolder;
	public bool adaptParentHeight;
	public Dictionary<FieldInfo, ParameterEditor> parameterEditors = new Dictionary<FieldInfo, ParameterEditor>();

	void Start()
	{
		if (uiHolder != null)
		{
			ParameterEditor.holder = uiHolder;
		}
		target = Parameters.Instance;
		FieldInfo[] infos = typeof(Parameters).GetFields();
		foreach (FieldInfo info in infos)
		{
			object[] attrs = info.GetCustomAttributes(true);
			foreach (object attr in attrs)
			{
				if (attr is ParameterAttribute)
				{
					ParameterAttribute parameterAttr = attr as ParameterAttribute;
					if (parameterAttr != null)
					{
						string name;
						if (parameterAttr.GetName() != "")
						{
							name = parameterAttr.GetName();
                        }
						else
						{
							name = info.Name;
						}
						Debug.Log("Name : " + name);
						Debug.Log("Type : " + info.FieldType.ToString());
						bool found = false;
						switch (info.FieldType.ToString())
						{
							case "System.String":
								parameterEditors.Add(info, new StringParameterEditor(name));
								found = true;
								break;
							case "System.Single":
								parameterEditors.Add(info, new FloatParameterEditor(name, (float)info.GetValue(target)));
								found = true;
								break;
							case "System.Int32":
								parameterEditors.Add(info, new IntParameterEditor(name, (int)info.GetValue(target)));
								found = true;
								break;
							case "System.Boolean":
								parameterEditors.Add(info, new BooleanParameterEditor(name, (bool)info.GetValue(target)));
								found = true;
								break;
						}
						if (found == false && info.FieldType.BaseType == typeof(System.Enum))
						{
							parameterEditors.Add(info, new EnumParameterEditor(name, info.FieldType, info.GetValue(target).ToString()));
							found = true;
						}
                    }
					break;
				}
			}
		}
	}

	void Update()
	{
		float height = 0;
		foreach (KeyValuePair<FieldInfo, ParameterEditor> kvp in parameterEditors)
		{
			float heightDiff = kvp.Value.editorTransform.rect.height;

			kvp.Value.editorTransform.anchorMax = Vector2.one;
			kvp.Value.editorTransform.anchorMin = Vector2.up;
            kvp.Value.editorTransform.offsetMax = new Vector2(0, height);
			height -= heightDiff;
			kvp.Value.editorTransform.offsetMin = new Vector2(0, height);
			height -= 5;

			kvp.Key.SetValue(target, kvp.Value.GetValue());
		}
		if (adaptParentHeight)
		{
			RectTransform trans = (uiHolder.transform as RectTransform);
			float currentHeight = trans.rect.height;
			trans.offsetMax = new Vector2(trans.offsetMax.x, trans.offsetMin.y - height);
		}
	}

	void OnDestroy()
	{
		target.Save();
	}
	/*
	private Vector2 scrollPos;
	public int width;
	public int height;
	void OnGUI()
	{
		return;
		if (SceneManager.GetActiveScene().name != "MainMenu")
		{
			return;
		}

		GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(width), GUILayout.MinHeight(height) };
		
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.Space(50);
		GUILayout.BeginHorizontal(style);
		GUILayout.Label("Left input", style);
		if (GUILayout.Button("Brake", options))
		{
			Parameters.Instance.leftInput = INPUT_TYPE.BRAKE;
		}
		if (GUILayout.Button("Boost", options))
		{
			Parameters.Instance.leftInput = INPUT_TYPE.BOOST;
		}
		GUILayout.Label(Parameters.Instance.leftInput.ToString(), style);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Right input", style);
		if (GUILayout.Button("Brake", options))
		{
			Parameters.Instance.rightInput = INPUT_TYPE.BRAKE;
		}
		if (GUILayout.Button("Boost", options))
		{
			Parameters.Instance.rightInput = INPUT_TYPE.BOOST;
		}
		GUILayout.Label(Parameters.Instance.rightInput.ToString(), style);
		GUILayout.EndHorizontal();
		GUILayout.Label("Boost delay", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.boostDelay.ToString(), options), out Parameters.Instance.boostDelay);
		GUILayout.Label("Boost Strength", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.boostStrength.ToString(), options), out Parameters.Instance.boostStrength);
		GUILayout.Label("Negative speed", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.maxVelocity.x.ToString(), options), out Parameters.Instance.maxVelocity.x);
		GUILayout.Label("Max speed", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.maxVelocity.y.ToString(), options), out Parameters.Instance.maxVelocity.y);
		GUILayout.Label("Acceleration", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.accelerationSpeed.ToString(), options), out Parameters.Instance.accelerationSpeed);
		GUILayout.Label("Brake", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.brakeSpeed.ToString(), options), out Parameters.Instance.brakeSpeed);

		GUILayout.Space(25);
		Parameters.Instance.speedScores = GUILayout.Toggle(Parameters.Instance.speedScores, "Speed as score", options);
		Parameters.Instance.platformScores = GUILayout.Toggle(Parameters.Instance.platformScores, "Platforms as score", options);
		GUILayout.Label("Max platforms", style);
		int.TryParse(GUILayout.TextField(Parameters.Instance.maxPlatformsCount.ToString(), options), out Parameters.Instance.maxPlatformsCount);
		GUILayout.Label("Platform distance", style);
		float.TryParse(GUILayout.TextField(Parameters.Instance.platformDistance.ToString(), options), out Parameters.Instance.platformDistance);

		if (GUILayout.Button("Start", options))
		{
			SceneManager.LoadScene("MainScene");
		}

		if (GUILayout.Button("Save", options))
		{
			Parameters.Instance.Save();
		}

		GUILayout.EndScrollView();
	}

	void OnDestroy()
	{
	}*/
}
