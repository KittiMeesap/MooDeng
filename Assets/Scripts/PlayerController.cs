using System.Collections;
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

    public float slideForce = 10f; // ความแรงในการสไลด์
    public float slideDuration = 0.5f; // เวลาที่ใช้ในการสไลด์
    private bool isSliding = false;
    private BoxCollider2D boxCollider;

    private bool isPoweredUp = false;
    private float powerUpTimer = 0f;

    private SpriteRenderer spriteRenderer; // ใช้สำหรับเปลี่ยนสีของ SpriteRenderer
    private Color originalColor; // เก็บสีเดิมของผู้เล่น

    [Header("Audio Sources")]
    [SerializeField] private AudioSource walkAudioSource; // สำหรับเสียงเดิน
    [SerializeField] private AudioSource slideAudioSource; // สำหรับเสียงสไลด์
    public bool playerisWalk = false; // สถานะการเดิน




    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        footEmissions = footsteps.emission;

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(2.080235f, 1.530562f); // ขนาดเริ่มต้นของ Collider
        boxCollider.offset = new Vector2(-0.05012035f, 0.0332014f); // Offset เริ่มต้น

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found in child objects!");
        }
    }

    private void Update()
    {
        isGroundedBool = IsGrounded();

        // ตรวจสอบสถานะการเดิน
        playerisWalk = moveX != 0 && isGroundedBool && !isSliding;

        // เล่นหรือหยุดเสียงเดิน
        if (playerisWalk && !walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
        }
        else if (!playerisWalk && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }

        if (isGroundedBool)
        {
            canDoubleJump = true;
            moveX = playerInput.actions["MoveLeft"].ReadValue<float>() * -1f + playerInput.actions["MoveRight"].ReadValue<float>();

            if (playerInput.actions["Jump"].triggered)
            {
                Jump(jumpForce);
            }
        }
        else
        {
            if (canDoubleJump && playerInput.actions["Jump"].triggered)
            {
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

        if (playerInput.actions["Slide"].triggered && !isSliding)
        {
            StartCoroutine(Slide());
        }

        if (playerInput.actions["Slide"].IsPressed() && isSliding)
        {
            rb.velocity = new Vector2(moveX * slideForce, rb.velocity.y);
        }

        if (playerInput.actions["Slide"].WasReleasedThisFrame() && isSliding)
        {
            StopSlide();
        }

        if (isPoweredUp)
        {
            powerUpTimer -= Time.deltaTime;

            if (powerUpTimer <= 0)
            {
                DeactivatePowerUp();
            }
        }
    }


    public void ActivatePowerUp(float duration)
    {
        isPoweredUp = true;
        powerUpTimer = duration;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }
    }

    private void DeactivatePowerUp()
    {
        isPoweredUp = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public bool IsPoweredUp()
    {
        return isPoweredUp;
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        playeranim.SetBool("isSliding", true);

        // เปลี่ยนขนาดและตำแหน่งของ Collider เมื่อสไลด์
        boxCollider.size = new Vector2(2.080235f, 0.7642583f);
        boxCollider.offset = new Vector2(-0.05012035f, -0.3499504f);

        rb.velocity = new Vector2(moveX * slideForce, rb.velocity.y);

        // เล่นเสียงสไลด์
        if (!slideAudioSource.isPlaying)
        {
            slideAudioSource.Play();
        }

        yield return new WaitForSeconds(slideDuration);

        if (!playerInput.actions["Slide"].IsPressed())
        {
            StopSlide();
        }
    }


    private void StopSlide()
    {
        isSliding = false;
        playeranim.SetBool("isSliding", false);

        // คืนขนาดและตำแหน่งของ Collider เป็นขนาดปกติ
        boxCollider.size = new Vector2(2.080235f, 1.530562f);
        boxCollider.offset = new Vector2(-0.05012035f, 0.0332014f);

        // หยุดเสียงสไลด์
        if (slideAudioSource.isPlaying)
        {
            slideAudioSource.Stop();
        }
    }


    private void FixedUpdate()
    {
        if (!isSliding)
        {
            moveX = playerInput.actions["MoveLeft"].ReadValue<float>() * -1f + playerInput.actions["MoveRight"].ReadValue<float>();
            rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
        }
    }

    private void Jump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        playeranim.SetTrigger("jump");
        SoundManager.instance.PlaySFX(SoundManager.instance.playerJumpClip);
    }

    private bool IsGrounded()
    {
        float radius = 0.15f;
        Vector2 position = new Vector2(groundCheck.position.x, groundCheck.position.y - 0.1f);

        Debug.DrawRay(position, Vector2.down * radius, Color.red);

        Collider2D hit = Physics2D.OverlapCircle(position, radius, groundLayer);
        return hit != null;
    }

    public void SetAnimations()
    {
        if (isSliding)
        {
            playeranim.SetBool("isSliding", true);
            playeranim.SetBool("run", false);
        }
        else
        {
            playeranim.SetBool("isSliding", false);

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
