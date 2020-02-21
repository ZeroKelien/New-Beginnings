using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Switch : MonoBehaviour
{
    [Header("On touching switch, move this object.")]
    public GameObject objectToMove = null;

    [Header("Target object will move this amount")]
    public Vector3 moveAmount = Vector3.up;
    [Header("Target object will move this speed")]
    public float moveSpeed = 2.0f;

    [Space()]
    [Header("Colors to change when On or Off")]
    public Color OnColor = Color.green;
    public Color OffColor = Color.red;

    [Header("If set, another object will change color to indicate on/off")]
    public GameObject indicatorLight = null;

    [Space()]
    [Header("Color of indicator line in scene view")]
    public Color debugLineColor = Color.yellow;

    
    private bool isOn = false;
    private MeshRenderer indicatorMR = null;
    private Vector3 targetStartPos = Vector3.zero;
    private Vector3 targetEndPos = Vector3.zero;
    private Coroutine movingFunction = null;
    public Coroutine moveCoroutine { get { return movingFunction; } }

    private Dictionary<GameObject, List<Switch>> switchGroups; //A searchable list of switch groups by target
    private List<Switch> myGroup = null;
    public static bool hasSetUpGroups = false;

    void Awake()
    {
        if (indicatorLight == null)
            indicatorLight = gameObject;

        indicatorMR = indicatorLight.GetComponent<MeshRenderer>();

        if (objectToMove == null)
            Debug.LogWarningFormat("{0} on {1}: No ObjToTrigger set.", GetType(), name);
        else
        {
            targetStartPos = objectToMove.transform.position;
            targetEndPos = targetStartPos + moveAmount;
        }//else

        if (!hasSetUpGroups)
            SetUpGroups();
    }//Awake
    void SetUpGroups()
    {
        //Create the searchable list
        switchGroups = new Dictionary<GameObject, List<Switch>>();

        //Find all switches
        Switch[] allSwitches = FindObjectsOfType<Switch>();

        //Loop through switches and group ones with the same target together
        for (int i = 0; i < allSwitches.Length; i++)
        {
            Switch theSwitch = allSwitches[i];
            GameObject target = theSwitch.objectToMove;
            
            //If there's already one in the list
            if (!switchGroups.ContainsKey(target))
                switchGroups.Add(target, new List<Switch>());

            switchGroups[target].Add(theSwitch);
            theSwitch.myGroup = switchGroups[target];
        }//for
    }//SetUpGoups

    void Update()
    {
        if (indicatorMR != null)
        {
            indicatorMR.material.color = isOn ? OnColor : OffColor;
        }//if
    }//
    void OnDrawGizmos()
    {
        Gizmos.color = debugLineColor;

        if (objectToMove != null)
        {
            Gizmos.DrawLine(transform.position, objectToMove.transform.position);

            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            //Draw destination point of target object
            Gizmos.color = Color.blue;
            if (Application.isPlaying)
            {
                start = isOn ? targetEndPos : targetStartPos;
                end = isOn ? targetStartPos : targetEndPos;
            }//if
            else
            {
                start = objectToMove.transform.position;
                end = objectToMove.transform.position + moveAmount;
            }//else

            Gizmos.DrawLine(start, end);
            Gizmos.DrawWireSphere(end, 0.25f);
        }//if
      
    }//OnDrawGizmos

    void OnTriggerEnter(Collider coll)
    {
        if (IsPlayer(coll.gameObject))
            DoSwitch();
    }//OnTriggerEnter

    void OnCollisionEnter(Collision collision)
    {
        if (IsPlayer(collision.collider.gameObject))
            DoSwitch();
    }//OnCollisionEnter

    IEnumerator MoveObject(GameObject movingObj, Vector3 destination)
    {
        while (movingObj.transform.position != destination)
        {
            movingObj.transform.position = Vector3.MoveTowards(movingObj.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }//while      
        movingFunction = null;
    }//MoveObject

    void ToggleSwitchGroup()
    {
        if (myGroup != null)
        {
            for (int i = 0; i < myGroup.Count; i++)
            {
                if (myGroup[i] != this)
                {
                    myGroup[i].isOn = !myGroup[i].isOn;
                }//if
            }//for
        }//if
    }

    public void StopMovingCoroutine()
    {
        if (movingFunction != null)
        {
            StopCoroutine(movingFunction);
            movingFunction = null;
        }//if
    }

    void StopGroupCoroutines()
    {
        if (myGroup != null)
        {
            for (int i = 0; i < myGroup.Count; i++)
            {
                if(myGroup[i].moveCoroutine != null)
                    myGroup[i].StopMovingCoroutine();
            }//for
        }//if
    }

    void DoSwitch()
    {
        if (objectToMove == null)
        {
            Debug.LogWarningFormat("{0} on {1}: No ObjToTrigger set.", GetType(), name);
            return;
        }//if

        if (objectToMove.activeSelf)
        {
            isOn = !isOn;
            ToggleSwitchGroup();

            StopGroupCoroutines();

            if (isOn)
            {
                //Move to endPos
                movingFunction = StartCoroutine(MoveObject(objectToMove, targetEndPos));
            }
            else
            {
                //Move back to startPos
                movingFunction = StartCoroutine(MoveObject(objectToMove, targetStartPos));
            }//else
        }//if
    }//OnCollect

    bool IsPlayer(GameObject targetObject)
    {
        Player player = targetObject.GetComponent<Player>();
        return player != null;
    }//CheckForPlayerDamage
    
}
