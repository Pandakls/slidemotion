using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip; 
    public float flashSpeed = 5f; // how quickly the damage image will show on the screen
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); // completly red


    Animator anim; // our animator reference
    AudioSource playerAudio;
    PlayerMovement playerMovement; //reference to a script
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;

	/*
	 * Will be called right at the begining
	 */
    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> ();
        playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
    }

	// Flash the damage image
    void Update ()
    {
        if(damaged)
        {
            damageImage.color = flashColour; //flash the image in red color
        }
        else
        {
			// faded back to transparent
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false; // damage back to false after showing the damage image
    }


    public void TakeDamage (int amount)
    {
        damaged = true;

        currentHealth -= amount;

        healthSlider.value = currentHealth;

        playerAudio.Play (); // play the audio

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;

        playerShooting.DisableEffects ();

        anim.SetTrigger ("Die"); // play the animation (our character dies)

        playerAudio.clip = deathClip;
        playerAudio.Play ();

        playerMovement.enabled = false; // no more mouvement
        playerShooting.enabled = false;
    }
}
