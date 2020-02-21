using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour 
{
    [Header("How much damage the player can take.")]
    public int startingHealth = 100;

    [Header("After damge, how many seconds invincible.")]
    public float invincibleLength = 0.75f;

    [Header("Speed in units per second")]
    public float playerSpeed = 4;

    [Header("Moves player relative to camera facing")]
    public bool cameraRelativeMovement = true;

    [Header("How high the player can jump (at normal gravity)")]
    public float jumpHeight = 2.0f;

    [Header("Can the player double jump?")]
    public bool doubleJumpEnabled = false;

    [Header("Jump button")]
    public KeyCode jumpButton = KeyCode.Space;

    [Header("Extra gravity")]
    [Range(0.0f, 1.0f)]
    public float extraGravity = 0.0f;

    [Space()]
    [Header("Clear points when player dies")]
    public bool clearPointsOnDeath = false;

    [HideInInspector]
    public float currentHealth = 0;

    private bool isGrounded = true;

    private bool isInvincible = false;
    
    private MeshRenderer meshRenderer = null;
    private Color meshColorCache;

    private Rigidbody rb = null;
    private Collider coll = null;
    private Vector3 input = Vector3.zero;
    private Camera m_Cam = null;

    private bool hasDoubleJumped = false;
	private Vector3[] feetOffset;
    //private bool jumpThisFrame = false;
    //private Vector3 gravityAdd = Vector3.zero;
    void Awake () 
	{
        currentHealth = startingHealth;
        meshRenderer = GetComponent<MeshRenderer>();
        meshColorCache = meshRenderer.material.color;

        //Add a rigidbody
        AddPhysics();

        m_Cam = Camera.main;
        isGrounded = true;
        hasDoubleJumped = false;
        
    }//Awake

    public void Heal(int percent)
    {
        float factor = (float)percent / 100.0f;
        currentHealth = Mathf.CeilToInt(Mathf.Min(currentHealth + startingHealth * factor, startingHealth));
    }//Heal

    public void AddToMaxHealth(int amount)
    {
        startingHealth += amount;
        currentHealth = startingHealth;
    }//ChangeMaxHealth

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

        /*if (coll.material == null || coll.material.name.ToLower().Contains("default"))
        {
            coll.material = new PhysicMaterial("NoFriction");
            coll.material.dynamicFriction = 0.01f;
            coll.material.staticFriction = 0.01f;
            coll.material.frictionCombine = PhysicMaterialCombine.Minimum;
            coll.material.bounciness = 0;
        }//if*/
		//Four feet, one on each corner for checking grounded

		feetOffset = new Vector3[4];
		float yOffs = -coll.bounds.extents.y + 0.05f;
		feetOffset[0] = new Vector3(coll.bounds.extents.x, yOffs , coll.bounds.extents.z);
		feetOffset[1] = new Vector3(-coll.bounds.extents.x, yOffs , coll.bounds.extents.z);
		feetOffset[2] = new Vector3(coll.bounds.extents.x, yOffs , -coll.bounds.extents.z);
		feetOffset[3] = new Vector3(-coll.bounds.extents.x, yOffs , -coll.bounds.extents.z);

    }//AddPhysics

    void Update () 
	{
        Controls();
	}//Update  

    private void Controls()
    {
        
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        input = new Vector3(horz, 0, vert);

        //jumpThisFrame = false;

		isGrounded = IsGrounded();//rb.velocity.y == 0;
        
        //Reset the double jump tracker, if needed
        if (hasDoubleJumped && isGrounded)
            hasDoubleJumped = false;


        if ((isGrounded || (!isGrounded && doubleJumpEnabled && !hasDoubleJumped) )&& Input.GetKeyDown(jumpButton))
        {
            Jump();
        }//if
    }//Controls

	bool IsGrounded()
	{
		float checkOffset = 0.1f;
		for (int i = 0; i < feetOffset.Length; i++)
		{
			//Debug.DrawRay(transform.position + feetOffset[i], Vector3.down * checkOffset, Color.red, checkOffset);
			RaycastHit[] hits =  Physics.RaycastAll(transform.position + feetOffset[i], Vector3.down * checkOffset, checkOffset);
			if (hits.Length > 0)
				return true;
		}//for
		return false;
	}//

    void Jump()
    {
        if (!isGrounded)
            hasDoubleJumped = true;

        Vector3 jumpVel = rb.velocity;

        jumpVel.y = 0;
        jumpVel += new Vector3(0, Mathf.Sqrt(-2.0f * Physics.gravity.y * (jumpHeight * 1.2f)), 0);
        rb.velocity = jumpVel;

    }//

    void FixedUpdate()
    {
       
        Vector3 cameraForward;
        Vector3 moveVector = Vector3.zero;

        if (cameraRelativeMovement)
        {
            // calculate camera relative direction to move:
            cameraForward = Vector3.Scale(m_Cam.transform.forward, new Vector3(1, 0, 1)).normalized;
            
            if (cameraForward == Vector3.zero)
            {
                //If it's a top-down camera
                moveVector = input.z * m_Cam.transform.up + input.x * m_Cam.transform.right;
            }//if
            else
            {
                moveVector = input.z * cameraForward + input.x * m_Cam.transform.right;
            }//else
        }//if
        else
            moveVector = input;
        
        
        rb.MovePosition(transform.position + moveVector.normalized * Time.deltaTime * playerSpeed);
        rb.velocity += new Vector3(0, -extraGravity, 0);

        
    }//FixedUpdate

    void LateUpdate()
    {
        Vector3 camTemp = Camera.main.transform.position;
        camTemp.y = transform.position.y;

        Vector3 vecFromCamera = transform.position - camTemp;


        transform.localRotation = Quaternion.LookRotation(vecFromCamera);
    }

    void CorrectWallStickyness()
    {
        // Get the velocity
        Vector3 horizontalMove = rb.velocity;
        // Don't use the vertical velocity
        horizontalMove.y = 0;
        // Calculate the approximate distance that will be traversed
        float distance = horizontalMove.magnitude * Time.fixedDeltaTime;
        // Normalize horizontalMove since it should be used to indicate direction
        horizontalMove.Normalize();
        RaycastHit hit;

        // Check if the body's current velocity will result in a collision
        if (rb.SweepTest(horizontalMove, out hit, distance))
        {
            // If so, stop the movement
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void OnDrawGizmosSelected()
	{
		if(Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			for (int i = 0; i < feetOffset.Length; i++)
			{
				Gizmos.DrawSphere(transform.position + feetOffset[i], 0.1f);

			}//for
		}//if
	}//

    public void PushPlayer(GameObject source)
    {
        //print("TODO: Push player away from the damage source?");
    }//PushPlayer

    public void FlashRed()
    {
        StartCoroutine(DoFlashRed());
    }//FlashRed

    private IEnumerator DoFlashRed()
    {
        int numFlashes = 5;
        //On and off pulse length
        float flashLength = invincibleLength / numFlashes / 2;

        Color flashColor = Color.red;
        flashColor.a = 0.1f;
        //Flash the player red so we know it took damage
        if (meshRenderer != null)
        {
            for(int i=0; i < numFlashes; i++)
            {
                //Turn red
                meshRenderer.material.color = flashColor;
                yield return new WaitForSeconds(flashLength);
                //Turn back
                meshRenderer.material.color = meshColorCache;
                yield return new WaitForSeconds(flashLength);
            }//for
        }//if
        
        yield return new WaitForEndOfFrame();
    }//FlashRed

    public void InvincibilityTimer()
    {
        StartCoroutine(DoInvincibilityTimer());
    }//InvincibilityTimer

    private IEnumerator DoInvincibilityTimer()
    {
        //MAKE PLAYER INVINCIBLE FOR invincibleLength TO AVOID JUGGLING
        isInvincible = true;
        yield return new WaitForSeconds(invincibleLength);
        isInvincible = false;
    }//DoInvincibilityTimer

    public void DoDamage(float amount, GameObject damager)
    {
        //Cant damage player while invincible, ignore it
        if (isInvincible)
            return;

        FlashRed();
        InvincibilityTimer();
        PushPlayer(damager);
        Debug.LogFormat("{0} deals {1} damage to player.", damager.name, amount);
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }//DoDamage

    public void Die()
    {
        print("PLAYER HAS DIED!");
        meshRenderer.enabled = false;

        Invoke("Restart", 2);
        enabled = false;
    }//Die

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }//Restart
}
