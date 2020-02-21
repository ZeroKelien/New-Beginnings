using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApproachPlayer : MonoBehaviour
{
	[Header("Range at which the enemy will follow the player.")]
	public float awakeRange = 10.0f;

	[Header("Speed that it runs to the player")]
	public float moveSpeed = 1.0f;

	[Header("Retarget every X meters")]
	public float retargetDistance = 5.0f;

	[Header("Retarget every X seconds")]
	public float retargetTime = 1.0f;

	/*[Header("Stop following if player is above by this much.")]
	public float stopOnPlayerHigher = 2.0f;*/

	private Player player = null;

	private bool isAwake = false;
	private bool hasTarget = false;
	private Vector3 targetPos;
	private float retargetTimer = 0;
	private Rigidbody rb = null;
	private Collider coll = null;
	private Vector3 newPos;

	void Awake()
	{
		player = FindObjectOfType<Player>();
		AddPhysics();
		newPos = transform.position;
	}//Awake

	void Start()
	{
		targetPos = transform.position;
		isAwake = false;
		hasTarget = false;
	}//Start

	private void AddPhysics()
	{
		rb = GetComponent<Rigidbody>();
		if(rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody>();
		}//if
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		rb.mass = 100;

		coll = GetComponent<Collider>();
		if (coll == null || coll.isTrigger || !coll.enabled)
		{
			Debug.LogWarningFormat("{0}: No enabled non-trigger collider specified on {1}, adding a sphere collider.", GetType(), gameObject.name);
			coll = gameObject.AddComponent<SphereCollider>();
		}//if
			

	}//AddPhysics

	void Update()
	{
		//Check for awake distance
		if (!isAwake)
		{
			if (Vector3.Distance(player.transform.position, transform.position) < awakeRange)
			{
				isAwake = true;
			}//if
		}//if

		newPos = transform.position;

		if(isAwake)
		{
			//If there's no target, get a new target 
			if (!hasTarget)
				targetPos = GetNewTarget();

			//If a target was gotten
			if(hasTarget)
			{
				//print("HAS TARGET");
				//Move towards target pos\
				newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

				retargetTimer -= Time.deltaTime;

				//If you reach target pos or run out of time, go to sleep
				if (transform.position == targetPos || retargetTimer <= 0)
				{
					hasTarget = false;
					isAwake = false;

					/*if (retargetTimer <= 0)
						print("TIMED OUT");
					else
						print("REACHED TARGET");*/
				}//if

			}//if


		}//if
	}//

	void FixedUpdate()
	{
		if (rb != null)
		{
			rb.MovePosition(newPos);
		}//if
		else
			transform.position = newPos;
	}//FixedUPdate

	Vector3 GetNewTarget()
	{
		hasTarget = false;
		Vector3 targetPos = Vector3.zero;

		//If enemy has LOS to the player, store the player's position as the new target
		if (HasLOS())
		{
			//Get the vector to the player
			Vector3 vecToPlayer = player.transform.position - transform.position;
			//Shorten it if the retargetDistance is closer than the player
			if (vecToPlayer.magnitude > retargetDistance)
				vecToPlayer = vecToPlayer.normalized * retargetDistance;

			//Set the target position
			targetPos = transform.position + vecToPlayer;
			targetPos.y = transform.position.y;

			hasTarget = true;
			retargetTimer = retargetTime;
		}//if
		else
		{
			print("NO LOS");
		}//else
		return targetPos;
	}//GetNewTarget

	bool HasLOS()
	{
		/*//If the player is too far above the enemy, there is no LOS
		if (player.transform.position.y - transform.position.y >= stopOnPlayerHigher)
		{
			print("PLAYER TOO HIGH, STOP FOLLOWING");
			return false;
		}*/

		RaycastHit[] hit = Physics.RaycastAll(transform.position, player.transform.position - transform.position, awakeRange);

		bool hasLOS = false;
		float closestDist = float.MaxValue;
		Collider closestHit = null;

		if (hit != null)
		{
			for (int i = 0; i < hit.Length; i++)
			{
				//print(hit[i].collider.name + " -> "+ i + "/" + hit.Length);

				//Don't hit yourself, your children, bullets or triggers
				if (IsIgnorable(hit[i]))
					continue;

				float dist = Vector3.Distance(hit[i].point, transform.position);

				if ( dist < closestDist)
				{
					closestDist = dist;
					closestHit = hit[i].collider;
					//print("New Closest" + closestHit.name );
				}//if

				//Check if you hit the player first, or something else
				Debug.DrawLine(transform.position, hit[i].point, Color.red);
			}//for
			if (closestHit != null && closestHit.gameObject == player.gameObject)
			{
				hasLOS = true;
			}//if

		}//if
		return hasLOS;
	}//

	bool IsIgnorable(RaycastHit hit)
	{
		if (IsBullet(hit.collider.gameObject))
		{
			print("IGNORED BULLET");
		}
		return hit.collider.gameObject == gameObject || hit.collider.isTrigger || IsChild(hit.collider.gameObject) || IsBullet(hit.collider.gameObject);
	}//IsIgnorable

	bool IsBullet(GameObject obj)
	{
		return obj.GetComponent<PlayerShoot.Bullet>() != null || obj.GetComponent<Turret.Bullet>() != null;
	}//bool

	bool IsChild(GameObject obj)
	{
		bool isChild = false;

		for (int i = 0; i < transform.childCount; i++)
		{
			if (obj == transform.GetChild(i))
			{
				isChild = true;
				print("ISCHILD");
				break;
			}//if
		}//for
		return isChild;
	}//IsChilds

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, awakeRange);
		Gizmos.color = Color.blue;

		if(hasTarget)
		{
			Gizmos.DrawWireSphere(targetPos, 0.2f);
			Gizmos.DrawLine(transform.position, targetPos);
		}//if
	}//OnDrawGizmos
}
