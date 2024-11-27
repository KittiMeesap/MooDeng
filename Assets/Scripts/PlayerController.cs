using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;


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

    // ขนาดและตำแหน่งของ Collider ในสถานะปกติและสถานะสไลด์
    [HideInInspector] Vector2 normalColliderSize = new Vector2(2.080235f, 1.530562f);
    [HideInInspector] Vector2 normalColliderOffset = new Vector2(-0.05012035f, 0.0332014f);
    [HideInInspector] Vector2 slideColliderSize = new Vector2(2.080235f, 0.7642583f);
    [HideInInspector] Vector2 slideColliderOffset = new Vector2(-0.05012035f, -0.3499504f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        footEmissions = footsteps.emission;

        // อ้างอิง BoxCollider2D และกำหนดขนาดเริ่มต้น
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = normalColliderSize; // ตั้งค่าเริ่มต้นให้เป็นขนาดปกติ
        boxCollider.offset = normalColliderOffset; // ตั้งค่าเริ่มต้นให้เป็นตำแหน่งปกติ

        // ค้นหา SpriteRenderer ที่อยู่ใน GameObject ลูก
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // เก็บสีเดิมของ Sprite
        }
        else
        {
            Debug.LogError("SpriteRenderer not found in child objects!");
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

        // เริ่มสไลด์เมื่อกดปุ่ม Slide และไม่อยู่ในสถานะสไลด์
        if (playerInput.actions["Slide"].triggered && !isSliding)
        {
            StartCoroutine(Slide());
        }

        // ถ้ากดปุ่มค้างไว้ให้สไลด์ต่อเนื่อง
        if (playerInput.actions["Slide"].IsPressed() && isSliding)
        {
            rb.velocity = new Vector2(moveX * slideForce, rb.velocity.y);
        }

        // หยุดสไลด์เมื่อปล่อยปุ่ม Slide
        if (playerInput.actions["Slide"].WasReleasedThisFrame() && isSliding)
        {
            StopSlide();
        }

        // ตรวจสอบสถานะเพิ่มพลัง
        if (isPoweredUp)
        {
            powerUpTimer -= Time.deltaTime;

            // หมดเวลาเพิ่มพลัง
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

        // เปลี่ยนสีผู้เล่นเป็นสีฟ้า
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }
    }

    private void DeactivatePowerUp()
    {
        isPoweredUp = false;

        // คืนสีผู้เล่นกลับเป็นสีเดิม
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public bool IsPoweredUp()
    {
        return isPoweredUp;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "killzone")
        {
            GameManager.instance.Death();
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        playeranim.SetBool("isSliding", true); // เริ่มอนิเมชันสไลด์

        // เปลี่ยนขนาดและตำแหน่งของ Collider เมื่อสไลด์
        boxCollider.size = slideColliderSize;
        boxCollider.offset = slideColliderOffset;

        // เริ่มการสไลด์โดยการตั้งค่า velocity ตามทิศทางที่กำลังเดินอยู่
        rb.velocity = new Vector2(moveX * slideForce, rb.velocity.y);

        // รอสักพักก่อนหยุดสไลด์ถ้าไม่ได้กดปุ่มค้าง
        yield return new WaitForSeconds(slideDuration);

        if (!playerInput.actions["Slide"].IsPressed())
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        isSliding = false;
        playeranim.SetBool("isSliding", false); // หยุดอนิเมชันสไลด์

        // คืนขนาดและตำแหน่งของ Collider เป็นขนาดปกติ
        boxCollider.size = normalColliderSize;
        boxCollider.offset = normalColliderOffset;

        // หยุดการเคลื่อนที่ในแนวนอนหลังการสไลด์
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        if (!isSliding) // ป้องกันไม่ให้เคลื่อนที่ตามปกติเมื่ออยู่ในสถานะสไลด์
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
    if (isSliding)
    {
        // ถ้ากำลังสไลด์ ให้แสดงอนิเมชันสไลด์อย่างเดียว
        playeranim.SetBool("isSliding", true);
        playeranim.SetBool("run", false);
    }
    else
    {
        // ถ้าไม่ได้สไลด์ แสดงอนิเมชันวิ่งหรือหยุดตามสถานะการเคลื่อนไหว
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
}
