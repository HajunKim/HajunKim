﻿using UnityEngine;

namespace Nightmare
{
    public class EnemyHealth : MonoBehaviour
    {
        public int startingHealth = 100;
        public float sinkSpeed = 2.5f;
        public int scoreValue = 10;
        public AudioClip deathClip;

        public int currentHealth;
        bool isSink = false;
        Animator anim;
        AudioSource enemyAudio;
        ParticleSystem hitParticles;
        CapsuleCollider capsuleCollider;
        EnemyMovement enemyMovement;
        public bool isBoss = false;
        float target_y;

        void Awake ()
        {
            anim = GetComponent <Animator> ();
            enemyAudio = GetComponent <AudioSource> ();
            hitParticles = GetComponentInChildren <ParticleSystem> ();
            capsuleCollider = GetComponent <CapsuleCollider> ();
            enemyMovement = this.GetComponent<EnemyMovement>();
        }

        void OnEnable()
        {
            currentHealth = startingHealth;
            // SetKinematics(false);
        }

        // private void SetKinematics(bool isKinematic)
        // {
        //     capsuleCollider.isTrigger = isKinematic;
        //     capsuleCollider.attachedRigidbody.isKinematic = isKinematic;
        // }

        void Update ()
        {
            if (IsDead() & isSink)
            {
                transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
                float y_diff = target_y - transform.position.y;
                
                // 5f;
                if (y_diff > 1f)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public bool IsDead()
        {
            return (currentHealth <= 0f);
        }

        public void TakeDamage (int amount, Vector3 hitPoint)
        {
            if (!IsDead())
            {
                enemyAudio.Play();
                currentHealth -= amount;

                if (IsDead())
                {
                    Death();
                }
                else if (!isBoss)
                {
                    enemyMovement.GoToPlayer();
                }
            }
            if (!isBoss)
            {        
                hitParticles.transform.position = hitPoint;
                hitParticles.Play();
            }
        }

        void Death ()
        {
            EventManager.TriggerEvent("Sound", this.transform.position);
            target_y = this.transform.position.y;
            anim.SetTrigger("Dead");

            enemyAudio.clip = deathClip;
            enemyAudio.Play();
            //StartSinking();
            if (isBoss)
            {
                foreach (Transform child in transform) 
                {
                    if (child.CompareTag ("BossParticle")) 
                    {
                        ParticleSystem effect = child.gameObject.GetComponent<ParticleSystem>();
                        effect.Stop();
                        
                    }
                }
            }
        }

        public void StartSinking ()
        {
            isSink = true;
            ScoreManager.score += scoreValue;
            GetComponent <UnityEngine.AI.NavMeshAgent> ().enabled = false;
            // SetKinematics(true);
        }

        public int CurrentHealth()
        {
            return currentHealth;
        }
    }
}