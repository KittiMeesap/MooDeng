using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGroundedBool = false;
    private bool canDoubleJump = false;

    public Animator playeranim;
    private float moveX;
    public bool isPaused = false;

    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmissions;

    public ParticleSystem ImpactEffect;
    private bool wasOnGround;

    public float fireRate = 0.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        footEmissions = footsteps.emission;

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput ไม่ได้ถูกกำหนดค่า!");
        }
    }

    private void Update()
    {
        isGroundedBool = IsGrounded();
        if (isGroundedBool)
        {
            canDoubleJump = true;
            moveX = playerInput.actions["MoveLeft"].ReadValue<float>() * -1f + playerInput.actions["MoveRight"].ReadValue<float>();

            if (playerInput.actions["Jump"].triggered)
            {
                Debug.Log("");
                Jump(jumpForce);
            }
        }
        else
        {
            if (canDoubleJump && playerInput.actions["Jump"].triggered)
            {
                Debug.Log("");
                Jump(doubleJumpForce);
                canDoubleJump = false; 
            }
        }

        SetAnimations();

        if (moveX != 0)
        {
            FlipSprite(moveX);
        }

        if (!wasOnGround && isGroundedBool)
        {
            ImpactEffect.gameObject.SetActive(true);
            ImpactEffect.Stop();
            ImpactEffect.transform.position = new Vector2(footsteps.transform.position.x, footsteps.transform.position.y - 0.2f);
            ImpactEffect.Play();
        }

        wasOnGround = isGroundedBool;
    }

    private void FixedUpdate()
    {
        moveX = playerInput.actions["MoveLeft"].ReadValue<float>() * -1f + playerInput.actions["MoveRight"].ReadValue<float>();
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }

    private void Jump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        playeranim.SetTrigger("jump");
    }

    private bool IsGrounded()
    {
        float radius = 0.15f; 
        Vector2 position = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.1f);

        Debug.DrawRay(position, Vector2.down * radius, Color.red);

        Collider2D hit = Physics2D.OverlapCircle(position, radius, groundLayer);
        bool isGrounded = hit != null;
        return isGrounded;
    }

    public void SetAnimations()
    {
        if (moveX != 0 && isGroundedBool)
        {
            playeranim.SetBool("run", true);
            footEmissions.rateOverTime = 35f;
        }
        else
        {
            playeranim.SetBool("run", false);
            footEmissions.rateOverTime = 0f;
        }
    }

    private void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "killzone")
        {
            GameManager.instance.Death();
        }
    }
}
