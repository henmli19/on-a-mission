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
        
        private bool canDash=true;
        private bool isDashing;
        [SerializeField] private float dashingPower = 24f;
        [SerializeField] private float dashingTime = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private TrailRenderer dashTrail;
        

        [Header("Ground Check Settings")] [SerializeField]
        private float groundCheckDistance = 0.1f; // length of raycast

        [SerializeField] private LayerMask groundLayer; // choose what counts as ground
        [SerializeField] private bool isGrounded = true;

        [SerializeField] private Animator _Animator;
        [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f);
        [SerializeField] public bool inputEnabled = true;
        
        public bool isShielded = false;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip dashSound;
        [SerializeField] private AudioClip walkingSound;
      
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

        }

        void Update()
        {
            
            if (beingGrabbed)
            {
                HandleGrabSpam();
                return; // Keine normale movements when grabbed
            }

            if (isDashing) return;
            CheckGrounded();


            if (!inputEnabled)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }

            if (!isDashing)
                HandleMovement();

            HandleJump();
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
               StartCoroutine(Dash());

            
            }
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
                audioSource.PlayOneShot(walkingSound);
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
                audioSource.PlayOneShot(jumpSound);
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

        public IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            float dashDirection = transform.localScale.x > 0 ? 1 : -1;
            rb.velocity = new Vector2(dashDirection * dashingPower * Time.deltaTime, 0f);
            dashTrail.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            dashTrail.emitting = false;
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
            audioSource.PlayOneShot(dashSound);
        }
    
        private Coroutine speedBoostCoroutine;

        public IEnumerator ApplySpeedBoost(float multiplier, float duration)
        {
            float originalSpeed = moveSpeed;
            moveSpeed *= multiplier;

            yield return new WaitForSeconds(duration);

            moveSpeed = originalSpeed;
        }
        
        public IEnumerator ApplyShield(float duration)
        {
            isShielded = true;
            // Visuelles Effekt hier spaeter.
    
            yield return new WaitForSeconds(duration);
    
            isShielded = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                BatteryHealthUI health = GetComponent<BatteryHealthUI>();

                if (health != null && isShielded == false)
                {
                    health.TakeDamage(1);
                }

                Destroy(other.gameObject);
            }
        }
        
        //Grabbing Minigame for the Sphere Enemy
        [Header("Grab Minigame")]
        public GameObject grabMessageUI; // Assign your "SPAM A/D" Text in Inspector
        private bool beingGrabbed = false;
        private int currentSpamCount = 0;
        private int requiredSpamCount = 10;
        private KeyCode lastPressed = KeyCode.None;
        private GameObject currentGrabber;
        
        public void StartGrabbingMinigame(int required, GameObject grabber)
        {
            beingGrabbed = true;
            requiredSpamCount = required;
            currentSpamCount = 0;
            currentGrabber = grabber;
    
            if (grabMessageUI != null) 
            {
                grabMessageUI.SetActive(true);
                // Debugging ...
                Debug.Log("Minigame Started: UI should be visible now.");
            }
            else
            {
                Debug.LogError("GrabMessageUI is NOT assigned in the Inspector!");
            }
        }

        public void StopGrabbingMinigame()
        {
            beingGrabbed = false;
            if (grabMessageUI != null) grabMessageUI.SetActive(false);
        }

        private void HandleGrabSpam()
        {
            // Check for alternating A and D presses
            if (Input.GetKeyDown(KeyCode.A) && lastPressed != KeyCode.A)
            {
                currentSpamCount++;
                lastPressed = KeyCode.A;
                transform.position += Vector3.left * 0.1f; // Visual shake
            }
            else if (Input.GetKeyDown(KeyCode.D) && lastPressed != KeyCode.D)
            {
                currentSpamCount++;
                lastPressed = KeyCode.D;
                transform.position += Vector3.right * 0.1f; // Visual shake
            }

            if (currentSpamCount >= requiredSpamCount)
            {
                if (currentGrabber != null)
                {
                    currentGrabber.GetComponent<SphereEnemy>().ReleasePlayer(this);
                }
            }
        }
        
        public bool IsBeingGrabbed()
        {
            return beingGrabbed; //me i than Shooting.cs a jam Grabbed a ja
        }
    }
}


