using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody2D rb;

    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

    }

    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
        //if (Input.GetKey(KeyCode.S)) movement += Vector3.down;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;
        if (movement != Vector3.zero) movement.Normalize();

        Vector3 newPosition = transform.position + movement * (speed * Time.deltaTime);
        transform.position = newPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        // Simple ground check (tag your ground as "Ground")
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }



}
