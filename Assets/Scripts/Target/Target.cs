﻿using System.Collections;
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
        public float ThetaScale = 0.01f;
        public float radius = 3f;
        int Size;
        LineRenderer LineDrawer;
        float Theta = 0f;
        AudioSource audioSource;
        public AudioClip vitalizedSound;
        //SoundModule soundModule;
        //public Color c1 = Color.white;
        //public Color c2 = new Color(1, 1, 1, 0);
        Light circleLight;
        float y = 0.0f;
        /*
        void Awake()
        {
            player = GameObject.FindGameObjectsWithTag ("Player");
            n_player = player.Length;
            targetColor = gameObject.GetComponent<Renderer>();
            LineDrawer = GetComponent<LineRenderer>();
        }
        */
        // Start is called before the first frame update

        void Start() 
        {
            player = GameObject.FindGameObjectsWithTag ("Player");
            n_player = player.Length;
            targetColor = gameObject.GetComponent<Renderer>();
            circleLight = GetComponent<Light> ();
            LineDrawer = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            //soundModule = GetComponent<SoundModule>();
            //LineDrawer.material = new Material(Shader.Find("Particles/Alpha Blended"));
            //LineDrawer.SetColors(Color.blue, Color.blue);

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
                if (!isVitalized)
                {
                    //Debug.Log("on");
                    //if() voice.on 
                    float value = SoundModule.Instance.GetPlayerNoteMappingValue();
                    //float value = soundModule.GetPlayerNoteMappingValue();
                    if (0.0f <= value && value < 0.0833f)
                    {
                        targetColor.material.color = new Color(255, 255, 255);
                    }
                    else if (0.0833f <= value && value < 0.166f)
                    {
                        targetColor.material.color = new Color(128, 255, 0);
                    }
                    else if (0.166f <= value && value < 0.25f)
                    {
                        targetColor.material.color = new Color(51, 102, 0);
                    }
                    else if (0.25f <= value && value < 0.315f)
                    {
                        targetColor.material.color = new Color(255, 0, 127);
                    }
                    else if (0.315f <= value && value < 0.416f)
                    {
                        targetColor.material.color = new Color(204, 0, 0);
                    }
                    else if (0.416 <= value && value < 0.5f)
                    {
                        targetColor.material.color = new Color(102, 0, 0);
                    }
                    else if (0.5f <= value && value < 0.583f)
                    {
                        targetColor.material.color = new Color(0, 204, 204);
                    }
                    else if (0.583f <= value && value < 0.666f)
                    {
                        targetColor.material.color = new Color(0, 0, 255);
                    }
                    else if (0.666f <= value && value < 0.75f)
                    {
                        targetColor.material.color = new Color(0, 0, 153);
                        isVitalized = true;
                        audioSource.clip = vitalizedSound;
                        audioSource.Play();
                        
                    }
                    else if (0.75f <= value && value < 0.833f)
                    {
                        targetColor.material.color = new Color(0, 0, 51);
                    }
                    else if (0.833f <= value && value < 0.916f)
                    {
                        targetColor.material.color = new Color(160, 160, 160);
                    }
                    else if (0.916f <= value && value < 1.0f)
                    {
                        targetColor.material.color = new Color(0, 0, 0);
                    }
                }
                //if(isVitalized)
                //{
                 //   y += 0.002f;
                //}
                //targetColor.material.color = new Color(255, 255, 255);
                //new Color(233, 79, 55);
                //Color(233f/255f, 79f/255f, 55f/255f);
                circleLight.enabled = true;
                LineDrawer.enabled = true; 
                Theta = 0f;
                Size = (int)((1f / ThetaScale) + 1f);
                LineDrawer.SetVertexCount(Size); 
                for(int i = 0; i < Size; i++){          
                    Theta += (2.0f * Mathf.PI * ThetaScale);         
                    float x = radius * Mathf.Cos(Theta);
                    float z = radius * Mathf.Sin(Theta);          
                    LineDrawer.SetPosition(i, new Vector3(x + transform.position[0], transform.position[1]-100.0f+y, z+transform.position[2]));
                 }
            }
            else
            {
                LineDrawer.enabled = false;
                circleLight.enabled = false;
                //Debug.Log("off");
            }
        }
    }
}
