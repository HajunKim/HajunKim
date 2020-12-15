using UnityEngine;
using System.Collections;

namespace Nightmare
{
    public class BossAttack : PausibleObject
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
        float particle_speed = 2.0f;

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
                    anim.SetBool("PlayerInRange",playerInRange);
                    // particle play !
                    foreach (Transform child in transform) 
                    {
                        if (child.CompareTag ("BossParticle")) 
                        {
                            ParticleSystem effect = child.gameObject.GetComponent<ParticleSystem>();
                            effect.Play();
                            effect.playbackSpeed = particle_speed;   
                        }
                    }
                    target_player = i;
                    break;
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
                    anim.SetBool("PlayerInRange",playerInRange);
                    foreach (Transform child in transform) 
                    {
                        if (child.CompareTag ("BossParticle")) 
                        {
                            ParticleSystem effect = child.gameObject.GetComponent<ParticleSystem>();
                            effect.Stop();
                            
                        }
                    }
                    break;
                }
            }
        }

        void Update ()
        {
            if (isPaused)
                return;
            
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
            if (n_death_player == n_player) anim.SetBool("PlayerDead",true);
        }

        void Attack ()
        {
            // If the player has health to lose...
            if (playerHealth[target_player].currentHealth > 0 && playerInRange)
            {
                // ... damage the player.
                foreach (Transform child in transform) 
                {
                    if (child.CompareTag ("BossAttackParticle")) 
                    {
                        child.gameObject.transform.position = player[target_player].transform.position;
                        ParticleSystem effect = child.gameObject.GetComponent<ParticleSystem>();
                        effect.Play();
                        
                    }
                }
                playerHealth[target_player].TakeDamage(attackDamage);
                // particle in player's position
            }
        }
    }
}