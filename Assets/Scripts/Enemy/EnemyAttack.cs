using UnityEngine;
using System.Collections;

namespace Nightmare
{
    public class EnemyAttack : PausibleObject
    {
        public float timeBetweenAttacks = 0.5f;
        public int attackDamage = 10;

        Animator anim;
        GameObject[] player;
        PlayerHealth[] playerHealth;
        EnemyHealth enemyHealth;
        bool playerInRange;
        float timer;
        int n_player;
        int target_player; // target player that will be damaged

        void Awake ()
        {
            // Setting up the references.
            player = GameObject.FindGameObjectsWithTag ("Player");
            n_player = player.Length;
            playerHealth = new PlayerHealth[n_player];
            for (int i = 0; i < n_player; i++)
            {
                playerHealth[i] = player[i].GetComponent<PlayerHealth>();
            }
            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent <Animator> ();

            StartPausible();
        }

        void OnDestroy()
        {
            StopPausible();
        }

        void OnTriggerEnter (Collider other)
        {
            for (int i=0; i < n_player; i++)
            {
                // If the entering collider is the player...
                if (other.gameObject == player[i])
                {
                    // ... the player is in range.
                    playerInRange = true;
                    target_player = i;
                }
            }
        }

        void OnTriggerExit (Collider other)
        {
            for (int i = 0; i < n_player; i++)
            {
                // If the exiting collider is the player...
                if (other.gameObject == player[i])
                {
                    // ... the player is no longer in range.
                    playerInRange = false;
                }
            }

        }

        void Update ()
        {
            if (isPaused)
                return;
            
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.CurrentHealth() > 0)
            {
                // ... attack.
                Attack ();
            }

            int n_death_player = 0;
            for (int i=0; i < n_player; i++)
            {
                // If the player has zero or less health...
                if (playerHealth[i].currentHealth <= 0)
                {
                    // ... tell the animator the player is dead.
                    n_death_player++;
                }
            }
            if (n_death_player == n_player) anim.SetTrigger("PlayerDead");
        }

        void Attack ()
        {
            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if (playerHealth[target_player].currentHealth > 0)
            {
                // ... damage the player.
                playerHealth[target_player].TakeDamage(attackDamage);
            }
        }
    }
}