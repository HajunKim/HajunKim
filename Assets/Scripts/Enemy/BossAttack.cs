using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        AudioSource audioSource;
        public AudioClip bossClip;
        public AudioClip enemyHurtClip; 
        public string targetNote;
        public int reflectiveDamage;
        public int randomNoteNumber;

        string[] notes = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};

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
            audioSource = GetComponent <AudioSource> ();
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
                    PlayBossSound();
                    // particle play !
                    // Random note select
                    randomNoteNumber = 4;
                    //randomNoteNumber = UnityEngine.Random.Range(0,notes.Length);
                    targetNote = notes[randomNoteNumber];
                    
                    // note color
                    var c_tuple = noteColorMapping[targetNote];
                    Color c = new Color(c_tuple.Item1/255.0f, c_tuple.Item2/255.0f, c_tuple.Item3/255.0f);
                    foreach (Transform child in transform) 
                    {
                        if (child.CompareTag ("BossParticle")) 
                        {
                            ParticleSystem effect = child.gameObject.GetComponent<ParticleSystem>();
                            var ps = effect.main;
                            ps.startColor = c;
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
                string note = SoundModule.Instance.GetPlayerNote();
                Debug.Log("Your note is " + note);
                note = note.Substring(0, note.Length - 1); //we are not using octave info
                if (SoundModule.Instance.GetPlayerDecibel() > 5 && String.Equals(note, targetNote))
                {
                    Debug.Log("Player is not attacked");
                    // Attack Boss
                    enemyHealth.TakeDamage(reflectiveDamage, this.transform.position);
                    return;
                }
                
                // ... damage the player.
                foreach (Transform child in transform) 
                {
                    if (child.CompareTag ("BossAttackParticle")) 
                    {
                        Debug.Log("Boss Attack ! target player is " + target_player);
                        child.gameObject.transform.position = player[target_player].transform.position;
                        ParticleSystem effect = child.gameObject.GetComponent<ParticleSystem>();
                        effect.Play();
                    }
                }
                playerHealth[target_player].TakeDamage(attackDamage);
                // particle in player's position
            }
        }

        void PlayBossSound()
        {            
            // Play BossClip
            audioSource.clip = bossClip;
            audioSource.Play();  
            // FMOD argument emission
        }

        public Dictionary<string, Tuple<float, float, float>> noteColorMapping = new Dictionary<string, Tuple<float, float, float>>()
        {
            { "C", new Tuple<float, float, float>(255.0f, 255.0f, 255.0f) }, // White
            { "C#", new Tuple<float, float, float>(128.0f, 255.0f, 0.0f) },
            { "D", new Tuple<float, float, float>(51.0f, 102.0f, 0.0f) },
            { "D#", new Tuple<float, float, float>(255.0f, 0.0f, 127.0f) },
            { "E", new Tuple<float, float, float>(204.0f, 0.0f, 0.0f) }, // Red
            { "F", new Tuple<float, float, float>(102.0f, 0.0f, 0.0f) },
            { "F#", new Tuple<float, float, float>(0.0f, 204.0f, 204.0f) },
            { "G", new Tuple<float, float, float>(0.0f, 0.0f, 255.0f) },  //Blue
            { "G#", new Tuple<float, float, float>(0.0f, 0.0f, 153.0f) },
            { "A", new Tuple<float, float, float>(0.0f, 0.0f, 51.0f) },
            { "A#", new Tuple<float, float, float>(160.0f, 160.0f, 160.0f) },
            { "B", new Tuple<float, float, float>(0.0f, 0.0f, 0.0f) },
        };
    }
}