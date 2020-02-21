using UnityEngine;
using System.Collections;

public class PathFollower : MonoBehaviour 
{
    public enum LoopType { PingPong, Loop}

    [Header("The nodes that form the path")]
    public Transform[] path = null;

    [Header("X Seconds to reach path end")]
    public float time = 3;
    
    [Header("How far above the path to hover")]
    public float hoverOffset = 0;

    [Header("What to do at path end")]
    public LoopType loopType = LoopType.Loop;

    [Header("Teleport to path start on frame 1")]
    public bool snapToPathStart = false;

    //[Header("Damage on player collide")]
    //public float damage = 0f;
    
    //private bool debugButton = false;
    private int currentNode = 0;
    private float speed = 5f;
    private Rigidbody rb = null;

    void Awake () 
	{
		if (path == null || path.Length == 0)
		{
			Debug.LogErrorFormat("{0}: No path specified for object {1}.", this.GetType(), name);
			this.enabled = false;
			return;
		}

        if (snapToPathStart)
        {
            if (path.Length > 0)
            {
                transform.position = path[0].position;
            }//if
        }//if

        UpdateTime();
    }//Awake

    void Start()
    {
        Follow();
    }//Start

    

    //------------------------------------
    public void Follow()
    {
        UpdateTime();
        currentNode = 0;
        StartCoroutine(FollowPathCoroutine());
        //isDone = false;
    }//Follow

    private void UpdateTime()
    {
        if (time > 0 && path != null && path.Length > 0)
            speed = TimeToSpeed();
    }
    //------------------------------------
    public IEnumerator FollowPathCoroutine()
    {
        while (currentNode < path.Length)
        {
            Vector3 destPos = path[currentNode].position + Vector3.up * hoverOffset;

            //Have we arrived at the next node?
            if (Vector3.Distance(destPos, transform.position) <= 0.05f)
                currentNode++;

            //Get the vector to the next node
            Vector3 moveVec = destPos - transform.position;

            //Normalize it and multiply by speed, make frame-independent
            moveVec = moveVec.normalized * speed * Time.deltaTime;

            if (moveVec.magnitude > Vector3.Distance(destPos, transform.position))
                transform.position = destPos;
            else
                transform.position += (Vector3)moveVec;

            yield return new WaitForEndOfFrame();
        }//while

        if (currentNode >= path.Length)
        {
            if (loopType == LoopType.Loop)
                Follow();
            else
            {
                ReversePath();
                Follow();
            }//else
        }//if

    }//FollowPathCoroutine

   /* void OnCollisionEnter(Collision collision)
    {
        if (damage <= 0)
            return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.DoDamage(damage, gameObject);
        }//if

    }//OnCollisionEnter*/

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

    public float SpeedToTime()
    {
        float length = GetPathLength();
        return length / speed;
    }//SpeedToTime

    public float TimeToSpeed()
    {
        float length = GetPathLength();

        //Add the distance from here to the start of the path, since we walk to the start and may not already be there
        length += Vector3.Distance(transform.position, path[0].position);

        return length / time;
    }//TimeToSpeed

    public float GetPathLength()
    {
        float length = 0;

        //Measure to the next one and add
        for (int i = 0; i < path.Length - 1; i++)
            length += Vector3.Distance(path[i].position, path[i + 1].position);

        return length;
    }//GetPathLength

    public void ReversePath()
    {
        Transform[] reversedPath = new Transform[path.Length];
        for (int i = 0; i < path.Length; i++)
            reversedPath[i] = path[path.Length - i-1];

        path = reversedPath;
    }//ReversePath

    void OnDrawGizmos()
    {
        if (path == null || path.Length <= 0)
            return;

        Gizmos.color = Color.green;

        bool badMeat = false;

        //Draw the middle
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == null)
            {
                Debug.LogWarning(string.Format("{0}.cs - On \"{1}\", path[{2}] is null. Remove it from the path array.", GetType(), name, i));
                badMeat = true;
            }//if
            else if (i >= 1)
                Gizmos.DrawLine(path[i].position, path[i - 1].position);
        }//for

        if (badMeat)
            return;

        //Draw the start and end
        if (path[path.Length - 1] != null)
            Gizmos.DrawWireSphere(path[path.Length - 1].position, 0.25f);

        Gizmos.DrawWireCube(path[0].position, Vector3.one * 0.25f * 2);

        //Draw the path offset
        Gizmos.color = Color.yellow;
        Vector3 pivotLoc = transform.position + Vector3.up * - hoverOffset;
        Gizmos.DrawSphere(pivotLoc, 0.25f);
        Gizmos.DrawLine(transform.position, pivotLoc);
    }//DoGizmos



    //------------------------------------
    void Update()
    {
       
    }//Update
}//PathFollower
