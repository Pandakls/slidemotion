using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;


    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    bool playerInRange; // true when the player is close enough to atack the enemy
    float timer;


    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag ("Player"); //locate the player
        playerHealth = player.GetComponent <PlayerHealth> (); // store a reference to the playerHealth script
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent <Animator> ();
    }


    void OnTriggerEnter (Collider other)
    {
		// if what we collact with is the player
        if(other.gameObject == player)
        {
            playerInRange = true;
        }
    }


    void OnTriggerExit (Collider other)
    {
		// if the thing that left the trigger is the player
        if(other.gameObject == player)
        {
            playerInRange = false;
        }
    }


    void Update ()
    {
		// how much time has occured
        timer += Time.deltaTime;

		// if enough time and player close enough and enemy not dead then we attack
        if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
        {
            Attack ();
        }

        if(playerHealth.currentHealth <= 0)
        {
            anim.SetTrigger ("PlayerDead");
        }
    }

	// ennemy attack
    void Attack ()
    {
		// reset timer
        timer = 0f;

		// if the player is alive
        if(playerHealth.currentHealth > 0)
        {
			// we give damage to the player
            playerHealth.TakeDamage (attackDamage);
        }
    }
}
