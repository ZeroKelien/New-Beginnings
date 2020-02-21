using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIPowerupTimer : MonoBehaviour 
{
	public enum Type { JumpHeight, SpeedIncrease }
	public Type timerType = Type.JumpHeight;
	public Text timerLabel = null;
	private Player player = null;
	public Text timerText = null;
	public Image timerImage = null;

	void Awake () 
	{
		if (player == null)
			player = GameObject.FindObjectOfType<Player>();

		if (timerText == null && timerImage == null)
			Debug.LogFormat("{0}: {1} script must be placed on a UI object with a 'Text' or 'Image' component attached", gameObject.name, GetType());	

		if(timerImage != null && timerImage.type != Image.Type.Filled)
			Debug.LogFormat("{1} on {0}: Attached Image component must be set to type 'Filled'", gameObject.name, GetType());	

		if (timerLabel != null)
		{
			timerLabel.text = (timerType == Type.JumpHeight) ? "Jump:" : "Speed:";
		}//if
	}//Awake

	void UpdateText(float timer)
	{
		if (timerText == null)
			return;

		timerText.text = Mathf.CeilToInt(timer).ToString();
	}//UpdateText

	void UpdateImage(float timer, float timerMax)
	{
		if (timerImage == null || timerImage.type != Image.Type.Filled)
			return;

		timerImage.fillAmount = timer / timerMax;

	}//UpdateImage

	void Update ()
	{
		float timer = (timerType == Type.JumpHeight) ? Powerup.jumpHeightTimer : Powerup.speedIncreaseTimer;
		float timerMax = (timerType == Type.JumpHeight) ? Powerup.jumpHeightTimerMax : Powerup.speedTimerMax;

		if (timer > 0)
		{
			ShowUI(true);
			UpdateText(timer);
			UpdateImage(timer, timerMax);
		}//if
		else
		{
			ShowUI(false);
		}//else
	}//Update

	void ShowUI(bool doShow)
	{
		if (timerLabel != null)
			timerLabel.enabled = doShow;

		if (timerText != null)
			timerText.enabled = doShow;

		if (timerImage != null)
			timerImage.enabled = doShow;
	}//
}//UIPoints
