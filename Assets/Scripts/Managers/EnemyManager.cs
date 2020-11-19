using UnityEngine;

namespace Nightmare
{
    public class EnemyManager : PausibleObject
    {
        private PlayerHealth playerHealth;
        public GameObject enemy;
        public float spawnTime = 3f;
        public Transform[] spawnPoints;
        public int n_enemy = 3;
        int n_count = 0;

        private float timer;
        //private int spawned = 0;

        void Start ()
        {
            timer = spawnTime;
        }

        void OnEnable()
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            StartPausible();
        }

        void OnDestroy()
        {
            StopPausible();
        }

        void Update()
        {
            if (isPaused || n_count >= n_enemy)
                return;

            if (timer <= 0f)
            {
                Spawn();
                timer = spawnTime;
                n_count++;
            }
            timer -= Time.deltaTime;
        }

        void Spawn ()
        {           
            // If the player has no health left...
            if(playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range(0, spawnPoints.Length-1);
            if (spawnPointIndex >= spawnPoints.Length) return;
            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        }
    }
}