using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour 
{
    public static int points = 0;

    [Header("On collection, add points to the score")]
    public int pointsToGive = 0;

    //[Header("On collection, if specified, disable linked object.")]
    private GameObject disableOnCollect = null;

    /*[Space()]
    [Header("Color of door-indicator line in scene view")]
    public Color debugColor = Color.yellow;
	*/

    /*void OnDrawGizmos()
    {
        Gizmos.color = debugColor;

        if (disableOnCollect != null && disableOnCollect.activeSelf)
        {
            Gizmos.DrawLine(transform.position, disableOnCollect.transform.position);
        }//if
    }//OnDrawGizmos
	*/

    void OnTriggerEnter(Collider coll)
    {
        if(IsPlayer(coll.gameObject))
            OnCollect();
    }//OnTriggerEnter

    void OnCollisionEnter(Collision collision)
    {
        if(IsPlayer(collision.collider.gameObject))
            OnCollect();
    }//OnCollisionEnter

    void OnCollect()
    {
        points += pointsToGive;
        OpenDoor();
        gameObject.SetActive(false);
    }//OnCollect

    bool IsPlayer(GameObject targetObject)
    {
        Player player = targetObject.GetComponent<Player>();
        return player != null;    
    }//CheckForPlayerDamage

    void OpenDoor()
    {
        if (disableOnCollect == null)
            return;

        disableOnCollect.SetActive(false);
    }//OpenDoor
}
