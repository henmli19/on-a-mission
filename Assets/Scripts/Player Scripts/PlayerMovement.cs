using System.Collections;
using UnityEngine;

namespace Player_Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {


        private Rigidbody2D rb;


        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 2f;

        [SerializeField] private float jumpForce = 4.8f;
        [SerializeField] private float dashForce = 6f;

        [Header("Ground Check Settings")] [SerializeField]
        private float groundCheckDistance = 0.1f; // length of raycast

        [SerializeField] private LayerMask groundLayer; // choose what counts as ground
        [SerializeField] private bool isGrounded = true;

        [SerializeField] private Animator _Animator;
        [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f);
        [SerializeField] public bool inputEnabled = true;
      



        private bool isDashing = false;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

        }

        void Update()
        {
            CheckGrounded();


            if (!inputEnabled)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }

            if (!isDashing)
                HandleMovement();

            HandleJump();
            HandleDash();
        }


    

    public void DisableControls()
        {
       
            inputEnabled = false; 
            rb.velocity = Vector2.zero; 
        }

        public void EnableControls()
        {
    
            inputEnabled = true; 
        }

        void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            // Move the player
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            if (horizontal != 0)
            {
                _Animator.SetBool("isRunning", true);
            }
            else
            {
                _Animator.SetBool("isRunning", false);
            }

            if (horizontal > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (horizontal < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); 
        }


        void HandleJump()
        {
            if (isGrounded && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
            }

            if (isGrounded )
            {
                _Animator.SetBool("isJumping", false);
            }
            else
            {
                _Animator.SetBool("isJumping", true);
                Debug.Log("isJumping = " + _Animator.GetBool("isJumping"));
            }

       

        }

        void HandleDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            {
                isDashing = true;
                rb.velocity = new Vector2(transform.localScale.x * dashForce, rb.velocity.y);

            
                Invoke(nameof(ResetDash), 0.3f);
            }
        }

        void ResetDash()
        {
            isDashing = false;
        }
        

        void CheckGrounded()
        {
            // Cast a ray straight down from the robot's position
            RaycastHit2D hit = Physics2D.Raycast(
                (Vector2)transform.position + groundCheckOffset,
                Vector2.down,
                groundCheckDistance,
                groundLayer
            );


            // Draw the ray in Scene view for debugging (green = hit, red = miss)
            Color rayColor = hit.collider != null ? Color.green : Color.red;
            Debug.DrawRay((Vector2)transform.position + groundCheckOffset, Vector2.down * groundCheckDistance, rayColor);


            // If the ray hits something on the ground layer, the robot is grounded
            isGrounded = hit.collider != null;
        }
    
        private Coroutine speedBoostCoroutine;

        public IEnumerator ApplySpeedBoost(float multiplier, float duration)
        {
            float originalSpeed = moveSpeed;
            moveSpeed *= multiplier;

            yield return new WaitForSeconds(duration);

            moveSpeed = originalSpeed;
        }
    }
}


