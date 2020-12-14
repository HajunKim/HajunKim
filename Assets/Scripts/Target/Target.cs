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


        

        // public Dictionary noteColorMapping; // byeol

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
            GameObject NoteHintChild1;
            GameObject NoteHintChild2;
            NoteHintChild1 = transform.GetChild(0).gameObject;
            NoteHintChild2 = NoteHintChild1.transform.GetChild(0).gameObject; // byeol
            Renderer NoteColor;

            player = GameObject.FindGameObjectsWithTag("Player");
            n_player = player.Length;
            targetColor = gameObject.GetComponent<Renderer>();
            circleLight = GetComponent<Light>();
            LineDrawer = GetComponent<LineRenderer>();
            audioSource = GetComponent<AudioSource>();
            //soundModule = GetComponent<SoundModule>();
            //LineDrawer.material = new Materia l(Shader.Find("Particles/Alpha Blended"));
            //LineDrawer.SetColors(Color.blue, Color.blue);

            NoteColor = NoteHintChild2.GetComponent<Renderer>(); // byeol
            int R = noteColorMapping[targetNote].Item1;
            int G = noteColorMapping[targetNote].Item2;
            int B = noteColorMapping[targetNote].Item3;
            Color C = new Color(R, G, B);
            //Debug.Log("R "+R+"G "+G+"B "+B);
            //NoteColor.material.color = C;
            NoteColor.material.SetColor("_MyBaseColor", C);
            //NoteColor.material.SetFloat("_Hue", noteValueMapping[targetNote]/12.0f);
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

        public Dictionary<string, float> noteValueMapping = new Dictionary<string, float>()
        {
            { "C", 1.0f },
            { "C#", 2.0f },
            { "D", 3.0f },
            { "D#", 4.0f },
            { "E", 5.0f },
            { "F", 6.0f },
            { "F#", 7.0f },
            { "G", 8.0f },
            { "G#", 9.0f },
            { "A", 10.0f },
            { "A#", 11.0f },
            { "B", 12.0f },
        };

        public Dictionary<string, Tuple<int, int, int>> noteColorMapping = new Dictionary<string, Tuple<int, int, int>>()
        {
            { "C", new Tuple<int, int, int>(255, 255, 255) }, // White
            { "C#", new Tuple<int, int, int>(128, 255, 0) },
            { "D", new Tuple<int, int, int>(51, 102, 0) },
            { "D#", new Tuple<int, int, int>(255, 0, 127) },
            { "E", new Tuple<int, int, int>(204, 0, 0) }, // Red
            { "F", new Tuple<int, int, int>(102, 0, 0) },
            { "F#", new Tuple<int, int, int>(0, 204, 204) },
            { "G", new Tuple<int, int, int>(0, 0, 255) },  //Blue
            { "G#", new Tuple<int, int, int>(0, 0, 153) },
            { "A", new Tuple<int, int, int>(0, 0, 51) },
            { "A#", new Tuple<int, int, int>(160, 160, 160) },
            { "B", new Tuple<int, int, int>(0, 0, 0) },

            // { "C", new Tuple<int, int, int>(205, 46, 42) }, // White
            // { "C#", new Tuple<int, int, int>(212, 94, 40) },
            // { "D", new Tuple<int, int, int>(216, 140, 50) },
            // { "D#", new Tuple<int, int, int>(222, 183, 62) },
            // { "E", new Tuple<int, int, int>(228, 226, 75) }, // Red
            // { "F", new Tuple<int, int, int>(178, 207, 67) },
            // { "F#", new Tuple<int, int, int>(100, 176, 55) },
            // { "G", new Tuple<int, int, int>(58, 130, 176) },  //Blue
            // { "G#", new Tuple<int, int, int>(33, 79, 173) },
            // { "A", new Tuple<int, int, int>(30, 26, 173) },
            // { "A#", new Tuple<int, int, int>(106, 34, 169) },
            // { "B", new Tuple<int, int, int>(164, 40, 108) },
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
                    Debug.Log("Current note : " + note);
                    if (note[note.Length - 1] < '2' || note[note.Length-1] > '5') return; // freq filtering by octave
                    note = note.Substring(0, note.Length - 1); //we are not using octave info
                    if (note.Length > 0)
                    {
                        float r = noteColorMapping[note].Item1;
                        float g = noteColorMapping[note].Item2;
                        float b = noteColorMapping[note].Item3;
                        
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
                                audioSource.loop = false;
                                audioSource.Play();
                                for (int i = 1; i <transform.childCount; i++)
                                {
                                    ParticleSystem effect = transform.GetChild(i).GetComponent<ParticleSystem>();
                                    effect.Play();
                                }
                                SoundModule.Instance.GamePhaseChange();
                            }
                            
                        }
                        Color c = new Color(r, g, b);
                        //float alpha = 0.5f+ 0.5f*(float)currentCnt / (float)targetActivateCnt; // 0 ~ 1
                        float alpha = 1.0f;
                        float new_r = alpha * r / 255.0f;
                        float new_g = alpha * g / 255.0f;
                        float new_b = alpha * b / 255.0f;
                        //Debug.Log("prev " + r + " " + g + " " + b);
                        //Debug.Log("new " + new_r + " " + new_g + " " + new_b);
                        //Color c = new Color((int)new_r, (int)new_g, (int)new_b);
                        targetColor.material.SetColor("_MyBaseColor", new Color(new_r,new_g,new_b,0));
                        //targetColor.material.SetColor("_BaseColor", new Color(r,g,b,0));
                        //Debug.Log("material name " + targetColor.material.name);
                        //targetColor.material.color = c;
                        //targetColor.material.SetFloat("_Hue", value);
                        //targetColor.material.SetVector("_Mycolor", new Vector4(r,g,b,0));
                        //targetColor.material.SetColor("_EmissionColor", c);
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
            else if (isVitalized)
            {
                LineDrawer.enabled = true;
                circleLight.enabled = true;
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
