using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
	[Header("Only shoot if line-of-sight to player?")]
	public bool shootOnLOSOnly = false;
	public float LOSLength = 10.0f;

    [Header("Shot damage to player")]
    public float damage = 10.0f;

    [Header("How fast the shot goes")]
    public float shotSpeed = 2.0f;

    [Header("One shot every x seconds")]
    public float shotTimer = 1.0f;

    [Header("Delay before first shot")]
    public float shotDelay = 0f;

    [Header("Turret spins this much after each shot")]
    public float rotateDegrees = 0f;

    [Header("What color the bullet is")]
    public Color shotColor = Color.red;

    [Header("How big the bullet is")]
    public float shotSize = 0.5f;

    [Header("If enemy dies, destroy all bullets")]
    public bool destroyBulletsOnDeath = true;

    private bool isRunning = false;
    private float maxShotDistance = 30;

	private Player player = null;

    private List<Bullet> bullets = null;

    void Awake()
    {
		player = FindObjectOfType<Player>();
        isRunning = true;
        bullets = new List<Bullet>();
        StartCoroutine(FiringLoop());
    }//Awake

    void Update()
    {
        //Keep bullet list up to date
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i] == null)
                bullets.RemoveAt(i);
        }//for
    }//Update

    public void Die()
    {
        if (destroyBulletsOnDeath)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i] != null)
                    Destroy(bullets[i].gameObject);
            }//for
        }//if
    }//Die

    private IEnumerator FiringLoop()
    {
        if (shotDelay > 0)
            yield return new WaitForSeconds(shotDelay);

        while (gameObject.activeSelf)
        {
			if (!shootOnLOSOnly || HasLOS())
			{
				Shoot();

				Invoke("Rotate", shotTimer / 2);
			

				if (shotTimer < 0.05f)
					shotTimer = 0.05f;

            
				yield return new WaitForSeconds(shotTimer);
			}//if
			else
				yield return new WaitForEndOfFrame();
        }//while

        yield return new WaitForEndOfFrame();
    }//FiringLoop

	bool HasLOS()
	{
		RaycastHit[] hit = Physics.RaycastAll(transform.position, player.transform.position - transform.position, LOSLength);
		//Debug.DrawRay(transform.position, player.transform.position - transform.position, awakeRange);

		bool hasLOS = false;
		float closestDist = float.MaxValue;
		Collider closestHit = null;

		if (hit != null)
		{
			for (int i = 0; i < hit.Length; i++)
			{
				//print(hit[i].collider.name + " -> "+ i + "/" + hit.Length);

				//Don't hit yourself or triggers
				if (hit[i].collider.gameObject == gameObject || hit[i].collider.isTrigger)
					continue;

				float dist = Vector3.Distance(hit[i].point, transform.position);

				if ( dist < closestDist)
				{
					closestDist = dist;
					closestHit = hit[i].collider;
					//print("New Closest" + closestHit.name );
				}//if

				//Check if you hit the player first, or something else
				//Debug.DrawLine(transform.position, hit[i].point, Color.red);
			}//for
			if (closestHit != null && closestHit.gameObject == player.gameObject)
			{
				hasLOS = true;
			}//if

		}//if
		return hasLOS;
	}//
    public void Shoot()
    {
        Bullet bull = Bullet.Spawn(transform.position + transform.forward, transform.forward, shotSpeed, shotColor, maxShotDistance, shotSize);
        bull.damage = damage;
        bull.shooter = gameObject;
        bull.Fire();
        bullets.Add(bull);
    }//Shoot

    public void Rotate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, rotateDegrees, 0));
    }//Rotate

    void OnDrawGizmos()
    {
        //Draw the debug gizmos in scene view unless the game has started
        if (isRunning)
            return;

        Vector3 lineEnd = transform.position + transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lineEnd, 0.1f);
        Gizmos.DrawLine(transform.position, lineEnd);

        if (shootOnLOSOnly)
        {
            Color transRed = Color.red;
            transRed.a = 0.25f;

            Gizmos.color = transRed;
            Gizmos.DrawSphere(transform.position, LOSLength);
        }//if
    }//OnDrawGizmos

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
            if(collision.gameObject.GetComponent<Bullet>() != null)
            {
                return;
            }//if

            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                //print("PLAYER IS HIT!!");
                player.DoDamage(damage, gameObject);
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
