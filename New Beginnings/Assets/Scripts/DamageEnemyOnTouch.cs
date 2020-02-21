using UnityEngine;
using System.Collections;

public class DamageEnemyOnTouch : MonoBehaviour
{
    [Header("Damage on collide with enemy")]
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
        CheckForEnemyDamage(coll.gameObject);
    }//OnTriggerEnter

    void OnTriggerStay(Collider coll)
    {
        CheckForEnemyDamage(coll.gameObject);
    }//OnTriggerEnter

    void OnCollisionEnter(Collision collision)
    {
        CheckForEnemyDamage(collision.gameObject);
    }//OnCollisionEnter

    void CheckForEnemyDamage(GameObject targetObject)
    {

        if (damage == 0)
            return;

        TakeDamageFromPlayer enemy = targetObject.GetComponent<TakeDamageFromPlayer>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
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
}//DamageEnemyOnTouch
