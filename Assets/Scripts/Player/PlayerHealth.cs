using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Nightmare
{
    public class PlayerHealth : MonoBehaviour
    {
        public int startingHealth = 100;
        public int currentHealth;
        public Slider healthSlider;
        public Image damageImage;
        public AudioClip deathClip;
        public AudioClip hurtClip;
        public float flashSpeed = 5f;
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
        public bool godMode = false;

        Animator anim;
        AudioSource playerAudio;
        PlayerMovement playerMovement;
        PlayerShooting playerShooting;
        public bool isDead;
        public bool damaged;

        void Awake()
        {
            // Setting up the references.
            anim = GetComponent<Animator>();
            playerAudio = GetComponent<AudioSource>();
            playerMovement = GetComponent<PlayerMovement>(); // just use script name
            playerShooting = GetComponentInChildren<PlayerShooting>();

            ResetPlayer();
        }

        public void ResetPlayer()
        {
            Debug.Log("reset!");
            // Set the initial health of the player.
            playerMovement.enabled = true;
            playerShooting.enabled = true;
            currentHealth = startingHealth;
            
            isDead = false;
            anim.SetBool("IsDead", isDead);
            anim.SetBool("IsWalking",true);
        }


        void Update()
        {
            // If the player has just been damaged...
            if (damaged)
            {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else
            {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }


        public void TakeDamage(int amount)
        {
            if (godMode || isDead) 
                return;

            else {

                // Set the damaged flag so the screen will flash.
                damaged = true;

                // Reduce the current health by the damage amount.
                currentHealth -= amount;

                // Set the health bar's value to the current health.
                healthSlider.value = currentHealth;

                // Play the hurt sound effect.
                // playerAudio.Play();
                playerAudio.clip = hurtClip;
                playerAudio.Play();

                // If the player has lost all it's health and the death flag hasn't been set yet...
                if (currentHealth <= 0 && !isDead)
                {
                    // ... it should die.
                    Death();
                }

                // Invoke("ResetPlayer", 5f);
                
            }


        }

        // void Death()
        // {
        //     // Set the death flag so this function won't be called again.
        //     isDead = true;

        //     // Turn off any remaining shooting effects.
        //     playerShooting.DisableEffects();

        //     // Tell the animator that the player is dead.
        //     anim.SetBool("i
        //     // Turn off the movement and shooting scripts.
        //     playerMovement.enabled = false;
        //     playerShooting.enabled = false;
        // }

        void Death()
        {
            // Set the death flag so this function won't be called again.

            // Tell the animator that the player is dead.
            isDead = true;
            anim.SetBool("IsDead", isDead);

            // Turn off any remaining shooting effects.
            playerShooting.DisableEffects();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play();

            float muteAmount = 5f;

            Invoke("ResetPlayer", muteAmount);
            SoundModule.Instance.MakePlayerMute(muteAmount);
        }



        public void RestartLevel()
        {
            EventManager.TriggerEvent("GameOver");
        }
    }
}