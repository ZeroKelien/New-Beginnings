using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerupNotification : MonoBehaviour 
{
	private Text textObj = null;
	private bool isShowingText = false;

	// Use this for initialization
	void Awake () 
	{
		textObj = gameObject.GetComponent<Text>();

		if (textObj == null)
		{
			Debug.LogFormat("{0}: {1} script must be placed on a UI object with a 'Text' component attached", gameObject.name, GetType());
			gameObject.SetActive(false);
			return;
		}//if

		textObj.text = "";
		textObj.enabled = false;
	}//Awake

	public void SetText(string text, float time)
	{
		StartCoroutine(ShowText(text, time));
	}//SetText

	IEnumerator ShowText(string text, float time)
	{
		//Queue up this message if one is already playing
		while (isShowingText)
			yield return new WaitForEndOfFrame();
		
		isShowingText = true;
		textObj.text = text;
		textObj.enabled = true;
		yield return new WaitForSeconds(time);
		textObj.text = "";
		textObj.enabled = false;
		isShowingText = false;
	}//ShowText
	
	// Update is called once per frame
	void Update () 
	{
		
	}//Update
}//
