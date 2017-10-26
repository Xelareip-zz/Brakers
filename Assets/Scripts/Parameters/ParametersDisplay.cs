using System;
using System.Collections;
using System.Collections.Generic;
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

public abstract class ParameterBase
{
	public abstract void Load();
	public abstract void Save();
	public abstract void Reset();

	public abstract void DeleteInstance();
}

public class ParametersDisplay : MonoBehaviour
{
	public GUIStyle style;

	public string parameterClassName = "Parameters";

	[NonSerialized]
	public ParameterBase target;
	public GameObject uiHolder;
	public bool adaptParentHeight;
	public Dictionary<FieldInfo, ParameterEditor> parameterEditors = new Dictionary<FieldInfo, ParameterEditor>();

	private void GetTarget()
	{
		foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
		{
			if (type.Name == parameterClassName)
			{
				PropertyInfo instanceMember = type.GetProperty("Instance");
				target = instanceMember.GetValue(null, null) as ParameterBase;
			}
		}
    }

	void Start()
	{
		if (uiHolder != null)
		{
			ParameterEditor.holder = uiHolder;
		}
		GetTarget();
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
		if (target == null)
		{
			return;
		}
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
		if (target != null)
		{
			target.Save();
		}
	}

	public void ResetAndReload()
	{
		target.Reset();
		target.DeleteInstance();
		target = null;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}