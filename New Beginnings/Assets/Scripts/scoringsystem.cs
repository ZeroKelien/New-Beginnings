using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoringsystem : MonoBehaviour
{
    public int points = 0;
    public AudioSource collectSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnGUI()
    {
        collectSound.Play();
        GUI.Label(new Rect(10,10,300,300), "Score : " + points);    

	}
}
