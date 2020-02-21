using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Bullet damage")]
    public float damage = 10.0f;

    [Header("How fast the shot goes")]
    public float shotSpeed = 5.0f;

    [Header("How far the bullets go before disappearing")]
    public float maxShotDistance = 50;

    [Header("Where the shot starts, offset from center")]
    public Vector3 shotOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;

    [Header("One shot every x seconds")]
    public float shotTimer = 0.1f;

    [Header("What color the bullet is")]
    public Color shotColor = Color.black;

    [Header("How big the bullet is")]
    public float shotSize = 0.5f;

    [Header("Button to Shoot")]
    public KeyCode shootButton = KeyCode.Mouse0;

    [Header("Allow holding the button")]
    public bool allowHold = true;
    
    private float timer = 0;
    private GameObject gunPos = null;
    void Awake()
    {
        //Spawn camera reticule
        //Hide it if it's not supposed to be shown
        timer = 0;
        SetPos();
      
    }//Awake

    public void SetPos()
    {
        if (gunPos == null)
        {
            gunPos = new GameObject("GUN POS");

            Quaternion playerRot = transform.rotation;
            transform.rotation = Quaternion.identity;

            gunPos.transform.position = transform.position + shotOffset;
            gunPos.transform.SetParent(transform);

            transform.rotation = playerRot;

        }//if
    }//

    void Update()
    {
        if ((!allowHold && Input.GetKeyDown(shootButton)) || (allowHold && Input.GetKey(shootButton)))
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = shotTimer;
                Shoot();
            }//if
        }//if
       
    }//Update

    public void Shoot()
    {
        Vector3 shotDirection = transform.forward;
       

        Bullet bull = Bullet.Spawn(gunPos.transform.position, shotDirection, shotSpeed, shotColor, maxShotDistance, shotSize);
        bull.damage = damage;
        bull.shooter = gameObject;
        bull.Fire();
    }//Shoot

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if(gunPos == null)
            Gizmos.DrawSphere(transform.position + shotOffset, 0.25f);
        else
            Gizmos.DrawSphere(gunPos.transform.position, 0.25f);
    }//
    

    public class Bullet : MonoBehaviour
    {
        public float speed = 1;
        public float maxDist = 0;
        public float damage = 1.0f;
        public Vector3 dir = Vector3.forward;
        public GameObject shooter = null;

        private Rigidbody rb = null;
        private Vector3 startPoint;

        public void Fire()
        {
            transform.position = startPoint;
            gameObject.SetActive(true);
        }//Fire

        void Update()
        {
            if (Vector3.Distance(transform.position, startPoint) > maxDist)
                Explode();
        }//Update

        void FixedUpdate()
        {
            rb.MovePosition(transform.position + dir * Time.deltaTime * speed * 2);
        }//FixedUpdate

        void OnCollisionEnter(Collision collision)
        {
            //Dont collide with the shooter of the bullet
            if (shooter != null && collision.gameObject == shooter)
            {
                return;
            }//if

            //Dont collide with other bullets
            if (collision.gameObject.GetComponent<Bullet>() != null)
            {
                return;
            }//if

            TakeDamageFromPlayer enemy = collision.gameObject.GetComponent<TakeDamageFromPlayer>();
            if (enemy != null)
            {
                //print("PLAYER IS HIT!!");
                enemy.TakeDamage(damage);
            }//if

            Explode();

        }//OnCollisionEnter

        private void Explode()
        {
            Destroy(gameObject);
        }//Explode

        private void AddPhysics()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }//if
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            rb.useGravity = false;

        }//AddPhysics

        public static Bullet Spawn(Vector3 startPos, Vector3 dir, float speed, Color color, float maxDist, float size)
        {
            GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulletObj.name = "Bullet";
            bulletObj.transform.position = startPos;
            bulletObj.transform.localScale = bulletObj.transform.localScale * size;

            MeshRenderer mr = bulletObj.GetComponent<MeshRenderer>();
            mr.material.color = color;

            Bullet bullet = bulletObj.AddComponent<Bullet>();
            bullet.AddPhysics();
            bullet.speed = speed;
            bullet.dir = dir;
            bullet.maxDist = maxDist;
            bullet.startPoint = startPos;

            bulletObj.SetActive(false);

            return bullet;
        }//Spawn
    }//Bullet
}
