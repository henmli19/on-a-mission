using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 4.8f;
    [SerializeField] private float dashForce = 6f;
   

    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isDashing = false;
  

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isDashing) HandleMovement();
        HandleJump();
        HandleDash();
        
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    void HandleJump()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                if (animator != null) animator.SetTrigger("Jump");
            }
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            rb.velocity = new Vector2(transform.localScale.x * dashForce, rb.velocity.y);
            if (animator != null) animator.SetTrigger("Dash");
            Invoke(nameof(ResetDash), 0.3f);
        }
    }



  

    void ResetDash()
    {
        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal == Vector2.up)
            isGrounded = true;
    }
}
