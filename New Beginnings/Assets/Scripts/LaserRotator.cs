using UnityEngine;
using System.Collections;

public class LaserRotator : MonoBehaviour 
{
    [Header("Does x damage to player with laser")]
    public float damage = 10.0f;

    [Header("How fast the laser rotates")]
    [Range(-10, 10)]
    public float rotationSpeed = 1.0f;

    [Header("How long the laser line is")]
    [Range(0, 20)]
    public float lineLength = 3;
        
    [Header("Color of laser line")]
    public Color lineColor = Color.red;

	[Header("Whether to stop or go through collision")]
	public bool stopOnCollide = true;
		
    //Private vars
    private Color colorCache;
    private LineRenderer lineRenderer = null;
    private bool isRunning = false;
    private float rot = 0;

    void Awake () 
	{
        //Cache some variables
        colorCache = lineColor;
        isRunning = true;

        //Ignore itself
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        //Add the line
        SetLinePos(transform.position, EndOfLineAtBeginning());
	}//Awake
    
	void Update () 
	{
        //Update the colors in case they changed
        if (lineColor != colorCache)
        {
            //lineRenderer.SetColors(lineColor, lineColor);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.material.color = lineColor;
            lineRenderer.material.SetColor("_EmissionColor", lineColor);
        }//if

        //Rotate the line
        rot += rotationSpeed;
		Vector3 rotatedLineEnd = RotatePointAroundPivot(EndOfLineAtBeginning(), transform.position, new Vector3(0,rot,0));


			//Raycast to check for a hit
	        RaycastHit hit;
			bool isHit = Physics.Raycast(transform.position, rotatedLineEnd - transform.position, out hit, Vector3.Distance(transform.position, rotatedLineEnd));


			if (isHit)
			{
				OnHit(hit);
				if (stopOnCollide)
				{
					rotatedLineEnd = hit.point;
				}//if
			}//if

		SetLinePos(transform.position, rotatedLineEnd);
    }//Update

    void OnHit(RaycastHit hit)
    {
        //What to do if the line hits something with a collider
        Player player = hit.transform.gameObject.GetComponent<Player>();
        if(player != null)
        {
            //print("PLAYER IS HIT!!");
            player.DoDamage(damage, gameObject);
        }//if
    }//OnHit

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) 
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }//RotatePointAroundPivot

    private void SetLinePos(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null)
        {
            //If the line object doesn't exist yet, create one
            DrawLine(transform.position, end, lineColor);
        }//if
        else
        {
            //Otherwise just update the line
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }//else
    }//SetLinePos

    private Vector3 EndOfLineAtBeginning()
    {
        return transform.position + transform.forward * lineLength;
    }//EndOfLineAtBeginning
    GameObject myLine = null;
    private void  DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        //If one exists already, destroy it first before caching the new one
        if (lineRenderer != null)
        {
            Destroy(lineRenderer);
        }//if
        
        //Set up a new line renderer
        myLine = new GameObject("Rotator laser line");
        myLine.transform.position = start;
        lineRenderer = myLine.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = color;
        lineRenderer.material.SetFloat("_Mode", 3);
        lineRenderer.material.SetColor("_EmissionColor", color);
        lineRenderer.material.EnableKeyword("_EMISSION");
        lineRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        //lineRenderer.SetColors(color, color);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        //lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);    
    }//DrawLine

    void OnDrawGizmos()
    {
        //Draw the debug gizmos in scene view unless the game has started
        if (isRunning)
            return;

        Vector3 lineEnd = EndOfLineAtBeginning();
        Gizmos.color = lineColor;
        Gizmos.DrawSphere(lineEnd, 0.1f);
        Gizmos.DrawLine(transform.position, lineEnd);
    }//OnDrawGizmos

    public void Die()
    {
        //Kill the laser
        this.enabled = false;
        lineRenderer.gameObject.SetActive(false);
        Destroy(lineRenderer.gameObject);
    }//Die
}//Rotator
