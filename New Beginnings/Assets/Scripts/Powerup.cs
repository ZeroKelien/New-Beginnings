using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
    //MaxHealth increases player max health by maxHealthPlus, and sets health to 100%
    //Healing increases player health by "healingAmountPercent"
    public enum Type { MaxHealth, Healing, JumpHeight, SpeedIncrease, GrantDoubleJump }

    [Header("Powerup type determines which vars are used")]
    public Type powerUpType = Type.Healing;

    [Space(10)]
    [Header("-- HEALTH-RELATED --")]
    [Header("Add to Max HP, if MaxHealth type")]
    public int maxHealthIncrease = 100;

    [Space()]
    [Range(0, 100)]
    [Header("HP to heal, if Healing type")]
    public int healingAmountPercent = 100;
   

    [Space(10)]
    [Header("-- JUMP-RELATED --")]
    [Header("Extra jump height, if JumpHeight type")]
    public float jumpHeightIncrease = 1.0f;

    [Space(10)]
    [Header("-- SPEED-RELATED --")]
    [Header("Added speed%, if SpeedIncrease type")]
    [Range(0, 200)]
    public int speedIncrease = 100;

    [Space(10)]
    [Header("-- PERMANENCE --")]
    [Header("Is Jump/Speed/DoubleJump powerup permanent?")]
    public bool isPermanent = false;
    [Header("How many seconds does it last, if not permanent?")]
    public float effectLength = 20.0f;
    
    private float initialJumpHeight = -1;
    private float initialSpeed = -1;

    private Player player = null;

    public static float jumpHeightTimer = 0;
	public static float jumpHeightTimerMax = 0;

    public static Coroutine jumpHeightTimerFunction = null;

    public static float speedIncreaseTimer = 0;
	public static float speedTimerMax = 0;

    public static Coroutine speedIncreaseTimerFunction = null;
	public static UIPowerupNotification notificationObject = null;
    void Awake()
    {
        player = FindObjectOfType<Player>();
        initialJumpHeight = player.jumpHeight;
        initialSpeed = player.playerSpeed;
        jumpHeightTimer = 0;
        speedIncreaseTimer = 0;

		if (notificationObject == null)
			notificationObject = FindObjectOfType<UIPowerupNotification>();
    }//Awake

	void DisplayMessage(string text)
	{
		if (notificationObject == null)
			return;

		notificationObject.SetText(text, 2.0f);
	}//DisplayMessage

    void Heal()
    {
		DisplayMessage("Healed " + healingAmountPercent + "% HP!");
        player.Heal(healingAmountPercent);
    }//Heal

    void IncreaseMaxHealth()
    {
		DisplayMessage(string.Format("Max Health +{0}!", maxHealthIncrease, player.startingHealth + maxHealthIncrease));
        player.AddToMaxHealth(maxHealthIncrease);
    }//IncreaseMaxHealth

    void IncreaseJumpHeight()
    {
        //If it's permanent, just increase the amount
        if (isPermanent)
        {
            initialJumpHeight += jumpHeightIncrease;
            player.jumpHeight += jumpHeightIncrease;
			DisplayMessage(string.Format("Jump height increased +{0}!", jumpHeightIncrease));
        }//if
        else
        {
			DisplayMessage(string.Format("Jump height +{0}% for {1} seconds!", speedIncrease, effectLength));

            //If not, set a timer to reverse the effect when it's done
            jumpHeightTimer += effectLength;
			jumpHeightTimerMax += effectLength;

            if (jumpHeightTimerFunction == null)
            {
                //Start timer, if one isn't already running
                jumpHeightTimerFunction = player.StartCoroutine(JumpHeightCountTime());
            }//
        }//else
        
    }//IncreaseJumpHeight

    IEnumerator JumpHeightCountTime()
    {
        player.jumpHeight = initialJumpHeight + jumpHeightIncrease;

        while (jumpHeightTimer > 0)
        {
            jumpHeightTimer -= Time.deltaTime;
            //print(jumpHeightTimer);
            yield return new WaitForEndOfFrame();
        }//while

        player.jumpHeight = initialJumpHeight;
        jumpHeightTimerFunction = null;
        jumpHeightTimer = 0;
		jumpHeightTimerMax = 0;
    }//JumpHeightCountTime

    void IncreaseSpeed()
    {

        //If it's permanent, just increase the amount
        if (isPermanent)
        {
            initialSpeed += speedIncrease;
            player.playerSpeed += player.playerSpeed * ((float)speedIncrease / 100.0f);
			DisplayMessage(string.Format("Speed increased +{0}%!", speedIncrease));
        }//if
        else
        {
			DisplayMessage(string.Format("Speed +{0}% for {1} seconds!", speedIncrease, effectLength));

            speedIncreaseTimer += effectLength;
			speedTimerMax += effectLength;
            if (speedIncreaseTimerFunction == null)
            {
                //Start timer, if one isn't already running
                speedIncreaseTimerFunction = player.StartCoroutine(SpeedCountTime());
            }//
        }//else
    }//IncreaseSpeed

    IEnumerator SpeedCountTime()
    {
        player.playerSpeed = initialSpeed + initialSpeed * ((float)speedIncrease / 100.0f);

        while (speedIncreaseTimer > 0)
        {
            speedIncreaseTimer -= Time.deltaTime;
            //print(speedIncreaseTimer);
            yield return new WaitForEndOfFrame();
        }//while

        player.playerSpeed = initialSpeed;

        speedIncreaseTimerFunction = null;
        speedIncreaseTimer = 0;
		speedTimerMax = 0;
    }//SpeedCountTime

    void GrantDoubleJump()
    {
		DisplayMessage("Player can now Double Jump!");
        player.doubleJumpEnabled = true;
    }//GrantDoubleJump

    void DoPowerup()
    {
        switch (powerUpType)
        {
            case Type.MaxHealth:
                IncreaseMaxHealth();
                break;
            case Type.Healing:
                Heal();
                break;
            case Type.JumpHeight:
                IncreaseJumpHeight();
                break;
            case Type.GrantDoubleJump:
                GrantDoubleJump();
                break;
            case Type.SpeedIncrease:
                IncreaseSpeed();
                break;
        }//switch
    }//DoPowerup

    

    void OnTriggerEnter(Collider coll)
    {
        if (IsPlayer(coll.gameObject))
            OnCollect();
    }//OnTriggerEnter

    void OnCollisionEnter(Collision collision)
    {
        if (IsPlayer(collision.collider.gameObject))
            OnCollect();
    }//OnCollisionEnter

    void OnCollect()
    {
        DoPowerup();
        gameObject.SetActive(false);
    }//OnCollect

    bool IsPlayer(GameObject targetObject)
    {
        return targetObject.GetComponent<Player>() != null;
    }//IsPlayer

  
}
