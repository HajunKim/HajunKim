using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Nightmare
{
    public class GameOverManager : MonoBehaviour
    {
        private PlayerHealth playerHealth;
        private Target target;
        private float restartDelay = 5f;
        
        Animator anim;
        float restartTimer;
        bool isEnding = false;
        float timer = 0.0f;
        int waitingTime = 2;

        LevelManager lm;
        private UnityEvent listener;

        void Awake()
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            target = FindObjectOfType<Target>();
            anim = GetComponent<Animator>();
        }
        void Update()
        {
            // If the target is activated
            if (target.isVitalized)
            {
                timer += Time.deltaTime;
                if (timer > waitingTime)
                {
                    anim.SetTrigger("Win");
                    isEnding = true;
                    //Action
                    timer = 0;
                }
            }
            // If the player has run out of health...

            // else if (playerHealth.currentHealth <= 0)
            // {
            //     // ... tell the animator the game is over.
            //     anim.SetTrigger("GameOver");
            //     isEnding = true;
            // }

            // if ending condition met 
            if (isEnding)
            {
                // .. increment a timer to count up to restarting.
                restartTimer += Time.deltaTime;

                // .. if it reaches the restart delay...
                if (restartTimer >= restartDelay)
                {
                    // .. then reload the currently loaded level.
                    Scene scene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(scene.name);
                }
            }
        }

        //void Awake ()
        //{
        //    playerHealth = FindObjectOfType<PlayerHealth>();
        //    anim = GetComponent <Animator> ();
        //    lm = FindObjectOfType<LevelManager>();
        //    EventManager.StartListening("GameOver", ShowGameOver);
        //}

        //void OnDestroy()
        //{
        //    EventManager.StopListening("GameOver", ShowGameOver);
        //}

        //void ShowGameOver()
        //{
        //    anim.SetBool("GameOver", true);
        //}

        //private void ResetLevel()
        //{
        //    ScoreManager.score = 0;
        //    LevelManager lm = FindObjectOfType<LevelManager>();
        //    Scene scene = SceneManager.GetActiveScene();
        //    SceneManager.LoadScene(scene.name);
        //    lm.LoadInitialLevel();
        //    anim.SetBool("GameOver", false);
        //    playerHealth.ResetPlayer();
        //}
    }
}