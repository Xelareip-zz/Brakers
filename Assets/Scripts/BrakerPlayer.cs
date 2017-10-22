using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum INPUT_TYPE
{
	BRAKE,
	BOOST,
	NONE
}

public enum BRAKE_TYPE
{
	FLAT,
	PROPORTIONNAL
}

public class BrakerPlayer : MonoBehaviour
{
	public GameObject endGame;
	public Text speedText;
	public Rigidbody2D rig;
	public INPUT_TYPE leftInput = INPUT_TYPE.BRAKE;
	public INPUT_TYPE rightInput = INPUT_TYPE.BRAKE;
	public BRAKE_TYPE brakeType = BRAKE_TYPE.PROPORTIONNAL;
	public float boostDelay;
	public float boostStrength;

	public Vector2 maxVelocity;
	public float minSpeed;
	public float maxSpeed;
	public float accelerationSpeed;
	public float brakeSpeed;

	public float lastBoost;

	public float currentSpeed;

	public bool speedScores;

	void Start()
	{
		leftInput = Parameters.Instance.leftInput;
		rightInput = Parameters.Instance.rightInput;
		boostDelay = Parameters.Instance.boostDelay;
		boostStrength = Parameters.Instance.boostStrength;
		minSpeed = Parameters.Instance.minSpeed;
		maxSpeed = Parameters.Instance.maxSpeed;
        accelerationSpeed = Parameters.Instance.accelerationSpeed;
		brakeSpeed = Parameters.Instance.brakeSpeed;
		speedScores = Parameters.Instance.speedScores;
	}

	void Update ()
	{
		transform.position += Vector3.up * currentSpeed * Time.deltaTime;

		float localAcceleration = accelerationSpeed;

		INPUT_TYPE inputType = INPUT_TYPE.NONE;

		bool hasInput = false;
		Vector2 inputPosition = Vector2.zero;

		if (Input.touchCount != 0)
		{
			hasInput = true;
			inputPosition = Input.GetTouch(0).position;
		}
		else if (Input.GetMouseButton(0))
		{
			hasInput = true;
			inputPosition = Input.mousePosition;
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			hasInput = true;
			inputPosition = new Vector2(0, 0);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			hasInput = true;
			inputPosition = new Vector2(Screen.width, 0);
		}

		if (hasInput)
		{
			if (inputPosition.x < Screen.width / 2.0f)
			{
				inputType = leftInput;
			}
			else
			{
				inputType = rightInput;
			}

			switch (inputType)
			{
				case INPUT_TYPE.BOOST:
					if (boostDelay + lastBoost < Time.time)
					{
						currentSpeed += boostStrength;
						lastBoost = Time.time;
					}
					break;
				case INPUT_TYPE.BRAKE:
					if (brakeType == BRAKE_TYPE.FLAT)
					{
						localAcceleration = -brakeSpeed;
					}
					else if (brakeType == BRAKE_TYPE.PROPORTIONNAL)
					{

						localAcceleration = -brakeSpeed * currentSpeed;
					}
					break;
				default:
					break;
			}
		}

		currentSpeed = Mathf.Clamp(currentSpeed + localAcceleration * Time.deltaTime, maxVelocity.x, maxVelocity.y);

		speedText.text = string.Format("{0:0.00}", currentSpeed);

		if (speedScores)
		{
			ScoreManager.Instance.score = Mathf.RoundToInt(Mathf.Max(currentSpeed, ScoreManager.Instance.score));
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "Wall")
		{
			endGame.SetActive(true);
			Destroy(gameObject);
		}
	}
}
