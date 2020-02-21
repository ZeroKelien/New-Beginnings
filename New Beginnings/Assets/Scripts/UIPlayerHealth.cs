using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIPlayerHealth : MonoBehaviour 
{
	private Player player = null;
	private Text textObj = null;
	private Image imageObj = null;

	void Awake () 
	{
        if (player == null)
            player = GameObject.FindObjectOfType<Player>();

		if(textObj == null)
        	textObj = gameObject.GetComponent<Text>();

		if(imageObj == null)
			imageObj = gameObject.GetComponent<Image>();

		if (textObj == null && imageObj == null)
			Debug.LogFormat("{0}: {1} script must be placed on a UI object with a 'Text' or 'Image' component attached", gameObject.name, GetType());	

		if(imageObj != null && imageObj.type != Image.Type.Filled)
			Debug.LogFormat("{1} on {0}: Attached Image component must be set to type 'Filled'", gameObject.name, GetType());	
		
	}//Awake

	void UpdateText()
	{
		if (textObj == null)
			return;

		if (player == null || player.currentHealth <= 0)
		{
			textObj.text = "Player has died.";
		}//
		else
			textObj.text = string.Format("Health: {0}", player.currentHealth);
	}//UpdateText

	void UpdateImage()
	{
		if (imageObj == null || imageObj.type != Image.Type.Filled)
			return;

		if (player != null)
			imageObj.fillAmount = player.currentHealth / player.startingHealth;

	}//UpdateImage

	void Update () 
	{
		UpdateText();
		UpdateImage();
	}//Update
}
