using UnityEngine;
using System.Collections;

public class CameraMouseOrbit : MonoBehaviour
{
    [Header("Distance from player")]
    public float startDistance = 5.0f;

    [Header("Limits when zooming with mouse wheel")]
    public float distanceMin = 3f;
    public float distanceMax = 15f;

    [Header("Rotation speeds")]
    public float xSpeed = 45.0f;
    public float ySpeed = 45.0f;
    
    [Header("Up/Down rotation limits")]
    public float yMinLimit = -20f;
    public float yMaxLimit = 50f;

    [Header("Center above/below target")]
    public float yOffset = 1.0f;

    [Header("Target camera points at (player default)")]
    public Transform target;

    [Header("Lock mouse to screen (esc to unlock in editor)")]
    public bool lockCursor = false;
    //public CursorLockMode cursorLockMode = CursorLockMode.None;

    //[Header("If disabled, camera snaps to near side of walls")]
    private bool allowWallClipping = true;
    private Rigidbody rb;

    float x = 0.0f;
    float y = 0.0f;

    void Awake()
    {
        if (target == null)
            target = FindObjectOfType<Player>().transform;
    }//Awake
    // Use this for initialization
    void Start()
    {

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rb = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;

        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * startDistance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            startDistance = Mathf.Clamp(startDistance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            Vector3 targetPos = target.position + Vector3.up * yOffset;

            if (!allowWallClipping)
            {
                RaycastHit hit;
                if (Physics.Linecast(targetPos, transform.position, out hit))
                {
                    startDistance -= hit.distance;
                }
            }//if
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -startDistance);
            Vector3 position = rotation * negDistance + targetPos;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}