using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
        public string targetNote;
        public string prevNote; // log of previous note
        private int currentCnt = 0; // count number of frames with success of targetNote 
        public int targetActivateCnt = 50; // number of frames for targetActivate

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
            player = GameObject.FindGameObjectsWithTag("Player");
            n_player = player.Length;
            targetColor = gameObject.GetComponent<Renderer>();
            circleLight = GetComponent<Light>();
            LineDrawer = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            //soundModule = GetComponent<SoundModule>();
            //LineDrawer.material = new Materia l(Shader.Find("Particles/Alpha Blended"));
            //LineDrawer.SetColors(Color.blue, Color.blue);

        }



        void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < n_player; i++)
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

        void OnTriggerExit(Collider other)
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

        Dictionary<string, Tuple<int, int, int>> noteColorMapping = new Dictionary<string, Tuple<int, int, int>>()
        {
            { "C", new Tuple<int, int, int>(255, 255, 255) },
            { "C#", new Tuple<int, int, int>(128, 255, 0) },
            { "D", new Tuple<int, int, int>(51, 102, 0) },
            { "D#", new Tuple<int, int, int>(255, 0, 127) },
            { "E", new Tuple<int, int, int>(204, 0, 0) },
            { "F", new Tuple<int, int, int>(102, 0, 0) },
            { "F#", new Tuple<int, int, int>(0, 204, 204) },
            { "G", new Tuple<int, int, int>(0, 0, 255) },
            { "G#", new Tuple<int, int, int>(0, 0, 153) },
            { "A", new Tuple<int, int, int>(0, 0, 51) },
            { "A#", new Tuple<int, int, int>(160, 160, 160) },
            { "B", new Tuple<int, int, int>(0, 0, 0) },
        };

        // Update is called once per frame
        void Update()
        {            
            if (playerInRange)
            {
                // light turn on
                circleLight.enabled = true;
                LineDrawer.enabled = true;
                Theta = 0f;
                Size = (int)((1f / ThetaScale) + 1f);
                LineDrawer.SetVertexCount(Size);
                for (int i = 0; i < Size; i++)
                {
                    Theta += (2.0f * Mathf.PI * ThetaScale);
                    float x = radius * Mathf.Cos(Theta);
                    float z = radius * Mathf.Sin(Theta);
                    LineDrawer.SetPosition(i, new Vector3(x + transform.position[0], transform.position[1] - 100.0f + y, z + transform.position[2]));
                }

                // target coloring
                if (!isVitalized)
                {
                    float value = SoundModule.Instance.GetPlayerNoteMappingValue();
                    string note = SoundModule.Instance.GetPlayerNote();
                    if (note[note.Length - 1] < '2' || note[note.Length-1] > '5') return; // freq filtering by octave
                    note = note.Substring(0, note.Length - 1); //we are not using octave info
                    if (note.Length > 0)
                    {
                        int r = noteColorMapping[note].Item1;
                        int g = noteColorMapping[note].Item2;
                        int b = noteColorMapping[note].Item3;
                        
                        if (String.Equals(note, targetNote))
                        {
                            if (currentCnt == 0) currentCnt = 1;
                            else
                            {
                                if (String.Equals(note, prevNote)) currentCnt++;
                                else currentCnt = 0;
                            }
                            Debug.Log(currentCnt);
                            if (targetActivateCnt == currentCnt)
                            {
                                Debug.Log("Target Note : " + targetNote + " activated!");
                                isVitalized = true;
                                audioSource.clip = vitalizedSound;
                                audioSource.Play();
                            }
                            
                        }
                        Color c = new Color(r, g, b);
                        c.a = 0.8f+ 0.2f*(float)currentCnt / (float)targetActivateCnt; // 0 ~ 1
                        targetColor.material.color = c;
                        //targetColor.material.SetFloat("_Metallic", alpha);

                        prevNote = note;
                    }
                    
                }
                //if(isVitalized)
                //{
                //   y += 0.002f;
                //}
                //targetColor.material.color = new Color(255, 255, 255);
                //new Color(233, 79, 55);
                //Color(233f/255f, 79f/255f, 55f/255f);
                
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
