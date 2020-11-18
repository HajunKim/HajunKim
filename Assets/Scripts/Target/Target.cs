using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Nightmare
{
    public class Target : MonoBehaviour
    {
        Renderer targetColor;
        public bool isVitalized = false;        
        GameObject[] player;
        bool playerInRange;
        int n_player;

        void Awake()
        {
            player = GameObject.FindGameObjectsWithTag ("Player");
            n_player = player.Length;
            targetColor = gameObject.GetComponent<Renderer>();
        }
        // Start is called before the first frame update



        void OnTriggerEnter (Collider other)
        {
            for (int i=0; i < n_player; i++)
            {
                // If the entering collider is the player...
                if (other.gameObject == player[i])
                {
                    // ... the player is in range.
                    playerInRange = true;
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
                    Debug.Log("off");
                    // ... the player is no longer in range.
                    playerInRange = false;
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(playerInRange)
            {
                Debug.Log("on");
                //if() voice.on 
                // GetPlayerNoteMappingValue
                
                targetColor.material.color = Color.blue;
                //new Color(233, 79, 55);
                //Color(233f/255f, 79f/255f, 55f/255f);
            }
            else
            {
                Debug.Log("off");
            }
        }
    }
}
