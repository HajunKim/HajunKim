using UnityEngine;

namespace Nightmare
{
    public class Enemyspawnonce : PausibleObject
    {
        public GameObject enemy;
        public Transform[] spawnPoints;

        //private int spawned = 0;


    



        public void Spawn ()
        {           

            int spawnPointIndex = Random.Range(0, spawnPoints.Length-1);
            if (spawnPointIndex >= spawnPoints.Length) return;
            Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        }
    }
}