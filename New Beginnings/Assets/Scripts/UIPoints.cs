using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class UIPoints : MonoBehaviour 
{
    private Text pointsCounter = null;

	void Awake () 
	{
        if (pointsCounter == null)
        {
            pointsCounter = gameObject.GetComponent<Text>();
        }//if
	}//Awake
	
	void Update () 
	{
        pointsCounter.text = string.Format("Points: {0}", Collectible.points);
	}//Update
}//UIPoints
