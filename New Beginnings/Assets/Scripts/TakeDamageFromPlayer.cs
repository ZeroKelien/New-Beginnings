using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageFromPlayer : MonoBehaviour
{
    [Header("How much health the enemy gets at the start")]
    public float maxHealth = 100;

    [Header("After damge, how many seconds invincible.")]
    public float invincibleLength = 0.75f;

    [Header("What color the enemy flashes when taking damage")]
    public Color damageColor = Color.red;

    private float currentHealth = 0;

    private MeshRenderer meshRenderer = null;
    private Color meshColorCache;
    private bool isInvincible = false;

    void Awake()
    {
        currentHealth = maxHealth;
        meshRenderer = GetComponent<MeshRenderer>();
        meshColorCache = meshRenderer.material.color;
    }//Awake

    public void TakeDamage(float damageAmount)
    {
        //Cant damage enemy while invincible, ignore it
        if (isInvincible)
            return;

        currentHealth -= damageAmount;

        FlashRed();

        if (currentHealth <= 0)
            StartCoroutine(DeathFunction());
    }//TakeDamage
   
    private IEnumerator DeathFunction()
    {
        gameObject.SendMessage("Die", SendMessageOptions.DontRequireReceiver);

        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }//DeathFunction

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
            for (int i = 0; i < numFlashes; i++)
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
    }//DoFlashRed

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
}//TakeDamageFromPlayer
