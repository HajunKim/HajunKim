using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using Rewired;

namespace Nightmare
{
    public class PlayerMovement : PausibleObject
    {
        public float speed = 6f;            // The speed that the player will move at.
        public float rotateSpeed = 4f;      // The spped of rotation
        public bool useMouse;               // use mouse or not(single play)

        
        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        PlayerHealth playerHealth;          // byeol
        CharacterController controller;
        private bool groundedPlayer;
        private Vector3 playerVelocity;
        private float gravityValue = -9.81f;

        float h;                            // up & down key amount.
        float v;                            // left & right key amount.
        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 100f;          // The length of the ray from the camera into the scene.

        // The Rewired player id of this character
        public int playerId = 0;
        private Player _player; // The Rewired Player

        void Awake ()
        {
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask ("Floor");

            // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
            _player = ReInput.players.GetPlayer(playerId);

            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length > 1) useMouse = false;
            
            // Set Cameramovement
            if (!useMouse)
            {
                GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
                camera.GetComponent<MyCameraFollow>().enabled = false;
            }

            // Set up references.
            anim = GetComponent <Animator> ();
            playerHealth = GetComponent <PlayerHealth> (); // byeol
            controller = GetComponent<CharacterController>();

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

            // Store the input axes.
            h = _player.GetAxis("Move Horizontal"); // get input by name or action id
            v = _player.GetAxis("Move Vertical");

            // Animate the player.
            Animating(h, v);

            // Move the player around the scene.
            // byeol adding
            Move (h, v);

            // Turn the player to face the mouse cursor.
            Turning ();

        }


        void Move (float h, float v)
        {
            if (h == 0 && v == 0) return;
            groundedPlayer = controller.isGrounded;
            float gravity = 0.0f;
            if (!groundedPlayer)
            {
                gravity = -9.8f;
            }

            movement = new Vector3(h, gravity, v);
            movement = movement.normalized * speed * Time.deltaTime;
            controller.Move(movement);

            
            gameObject.transform.forward = new Vector3(h, 0.0f, v);

            //gameObject.transform.Rotate(new Vector3(h, 0.0f, v));


        }


        void Turning ()
        {
            if (!useMouse)
            {
                if (h == 0 && v == 0)
                {
                    return; // no movement, no turn
                }
                Quaternion newRotation = Quaternion.LookRotation(movement);

                //gameObject.transform.rotation = newRotation;
            }
            else
            {
                // mouse input
                // Create a ray from the mouse cursor on screen in the direction of the camera.
                Debug.Log("Mouse" + Input.mousePosition);
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
                    Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

                    // Set the player's rotation to this new rotation.
                    gameObject.transform.rotation = newRotation;
                }
            }
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