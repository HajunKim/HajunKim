using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Nightmare
{
    public class PlayerMovement : PausibleObject
    {
        public float speed = 6f;            // The speed that the player will move at.
        public float rotateSpeed = 4f;      // The spped of rotation
        public bool useMouse;               // use mouse or not(single play)

        
        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
        PlayerHealth playerHealth;          // byeol
        float h;                            // up & down key amount.
        float v;                            // left & right key amount.
        string Hkeyname, Vkeyname;          // mapped key name of horizonal & vertical axis.(for multiplayer)
#if !MOBILE_INPUT
        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 100f;          // The length of the ray from the camera into the scene.
#endif

        void Awake ()
        {
#if !MOBILE_INPUT
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask ("Floor");
#endif
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length > 1) useMouse = false;
            
            // Set Cameramovement
            if (!useMouse)
            {
                GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                camera.GetComponent<MyCameraFollow>().enabled = false;
            }

            // Set key mappings
            Hkeyname = this.gameObject.name.Equals("Player") ? "Horizontal" : "Horizontal2P";
            Vkeyname = this.gameObject.name.Equals("Player") ? "Vertical" : "Vertical2P";

            // Set up references.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();
            playerHealth = GetComponent <PlayerHealth> (); // byeol

            StartPausible();
        }

        void OnDestroy()
        {
            StopPausible();
        }

        void FixedUpdate ()
        {
            if (isPaused)
                return;


            if (playerHealth.currentHealth > 0){
            // Store the input axes.
                h = CrossPlatformInputManager.GetAxisRaw(Hkeyname);
                v = CrossPlatformInputManager.GetAxisRaw(Vkeyname);
                
                // Move the player around the scene.
                // byeol adding
                Move (h, v);

                // Turn the player to face the mouse cursor.
                Turning ();

                // Animate the player.
                Animating (h, v);}
        }


        void Move (float h, float v)
        {
            // Set the movement vector based on the axis input.
            movement.Set (h, 0f, v);
            
            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement.normalized * speed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition (transform.position + movement);
        }


        void Turning ()
        {
#if !MOBILE_INPUT
            
            if (!useMouse)
            {
                if (h == 0 && v == 0)
                {
                    return; // no movement, no turn
                }
                Quaternion newRotation = Quaternion.LookRotation(movement);

                playerRigidbody.rotation = Quaternion.Slerp(playerRigidbody.rotation, newRotation, rotateSpeed * Time.deltaTime);
            }
            else
            {
                // mouse input
                // Create a ray from the mouse cursor on screen in the direction of the camera.
                Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Create a RaycastHit variable to store information about what was hit by the ray.
                RaycastHit floorHit;

                // Perform the raycast and if it hits something on the floor layer...
                if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
                {
                    // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                    Vector3 playerToMouse = floorHit.point - transform.position;

                    // Ensure the vector is entirely along the floor plane.
                    playerToMouse.y = 0f;

                    // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                    Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                    // Set the player's rotation to this new rotation.
                    playerRigidbody.MoveRotation(newRotatation);
                }
            }
#else

            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }


        void Animating (float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool ("IsWalking", walking);
        }
    }
}