using UnityEngine;
using System.Collections;

public class DamagePlayerOnTouch : MonoBehaviour
{
    public enum LoopType { PingPong, Loop }

    [Header("Damage on player collide")]
    public float damage = 10.0f;

    private Collider coll = null;

    private Color originalColor;

    void Awake()
    {
        AddPhysics();
    }//Awake

    void Start()
    {
    }//Start

    void OnTriggerEnter(Collider coll)
    {
        CheckForPlayerDamage(coll.gameObject);
    }//OnTriggerEnter

    void OnTriggerStay(Collider coll)
    {
        CheckForPlayerDamage(coll.gameObject);
    }//OnTriggerEnter

    void OnCollisionEnter(Collision collision)
    {
        CheckForPlayerDamage(collision.gameObject);

    }//OnCollisionEnter

    void CheckForPlayerDamage(GameObject targetObject)
    {
        if (damage == 0)
            return;

        Player player = targetObject.GetComponent<Player>();
        if (player != null)
        {
            player.DoDamage(damage, gameObject);
        }//if
    }//CheckForPlayerDamage

    private void AddPhysics()
    {
        if (coll == null)
        {
            coll = gameObject.GetComponent<Collider>();
        }//if

        if (coll == null || !coll.enabled)
        {
            Debug.LogWarningFormat("{0}: No enabled collider specified on {1}, adding a sphere collider.", GetType(), gameObject.name);
            coll = gameObject.AddComponent<SphereCollider>();
        }//if

    }//AddPhysics

    //------------------------------------
    void Update()
    {
       
    }//Update
}//DamagePlayerOnTouch
