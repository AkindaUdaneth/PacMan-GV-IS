using UnityEngine;

namespace IT23575608_CoreDeveloper
{
    /// <summary>
    /// A simple Player Controller for Pac-Man using the CharacterController component.
    /// Allows movement using WASD or the Arrow keys.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("How fast Pac-Man moves.")]
        public float moveSpeed = 5f;
        [Tooltip("How fast Pac-Man rotates to face the direction he is moving.")]
        public float turnSpeed = 15f;
        [Tooltip("Gravity applied to keep Pac-Man on the ground.")]
        public float gravity = 9.81f;

        private CharacterController characterController;

        void Start()
        {
            // Automatically grab the Character Controller attached to the GameObject
            characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            // 1. Get input from WASD or Arrow Keys
            // GetAxisRaw makes the movement snappy (0 or 1) rather than floaty
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // 2. Calculate movement direction based on input (ignore Y axis for horizontal movement)
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            // 3. If there is any input, move and rotate
            if (direction.magnitude >= 0.1f)
            {
                // Move Pac-Man
                characterController.Move(direction * moveSpeed * Time.deltaTime);

                // Rotate Pac-Man smoothly to face the direction of movement
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
            
            // 4. Apply basic gravity to keep him on the floor
            if (!characterController.isGrounded)
            {
                characterController.Move(Vector3.down * gravity * Time.deltaTime);
            }
        }
    }
}
