using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [Header("Text when you pick it up.")]
    public string pickupMessage = "YOU GOT A GUN!";

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

    private Player player = null;
    
    public static UIPowerupNotification notificationObject = null;
    void Awake()
    {
        player = FindObjectOfType<Player>();

        if (notificationObject == null)
            notificationObject = FindObjectOfType<UIPowerupNotification>();
    }//Awake

    void DisplayMessage(string text)
    {
        if (notificationObject == null)
            return;

        notificationObject.SetText(text, 2.0f);
    }//DisplayMessage

   

    void DoPowerup()
    {
        PlayerShoot gun = player.gameObject.GetComponent<PlayerShoot>();
        if (gun == null)
            gun = player.gameObject.AddComponent<PlayerShoot>();


        gun.damage = damage;
        gun.shotSpeed = shotSpeed;
        gun.maxShotDistance = maxShotDistance;
        gun.shotOffset = shotOffset;
        gun.shotTimer = shotTimer;
        gun.shotColor = shotColor;
        gun.shotSize = shotSize;
        gun.shootButton = shootButton;
        gun.allowHold = allowHold;

        gun.SetPos();

        DisplayMessage(pickupMessage);
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
