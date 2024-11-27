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

    public float slideForce = 10f; // �����ç㹡����Ŵ�
    public float slideDuration = 0.5f; // ���ҷ����㹡����Ŵ�
    private bool isSliding = false;
    private BoxCollider2D boxCollider;

    private bool isPoweredUp = false;
    private float powerUpTimer = 0f;

    private SpriteRenderer spriteRenderer; // ������Ѻ����¹�բͧ SpriteRenderer
    private Color originalColor; // ��������ͧ������

    // ��Ҵ��е��˹觢ͧ Collider �ʶҹл������ʶҹ���Ŵ�
    [HideInInspector] Vector2 normalColliderSize = new Vector2(2.080235f, 1.530562f);
    [HideInInspector] Vector2 normalColliderOffset = new Vector2(-0.05012035f, 0.0332014f);
    [HideInInspector] Vector2 slideColliderSize = new Vector2(2.080235f, 0.7642583f);
    [HideInInspector] Vector2 slideColliderOffset = new Vector2(-0.05012035f, -0.3499504f);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        footEmissions = footsteps.emission;

        // ��ҧ�ԧ BoxCollider2D ��С�˹���Ҵ�������
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = normalColliderSize; // ��駤�������������繢�Ҵ����
        boxCollider.offset = normalColliderOffset; // ��駤�������������繵��˹觻���

        // ���� SpriteRenderer �������� GameObject �١
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // ��������ͧ Sprite
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

        // �������Ŵ�����͡����� Slide �����������ʶҹ���Ŵ�
        if (playerInput.actions["Slide"].triggered && !isSliding)
        {
            StartCoroutine(Slide());
        }

        // ��ҡ�������ҧ��������Ŵ������ͧ
        if (playerInput.actions["Slide"].IsPressed() && isSliding)
        {
            rb.velocity = new Vector2(moveX * slideForce, rb.velocity.y);
        }

        // ��ش��Ŵ�����ͻ���»��� Slide
        if (playerInput.actions["Slide"].WasReleasedThisFrame() && isSliding)
        {
            StopSlide();
        }

        // ��Ǩ�ͺʶҹ�������ѧ
        if (isPoweredUp)
        {
            powerUpTimer -= Time.deltaTime;

            // �������������ѧ
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

        // ����¹�ռ��������տ��
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.blue;
        }
    }

    private void DeactivatePowerUp()
    {
        isPoweredUp = false;

        // �׹�ռ����蹡�Ѻ�������
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
        playeranim.SetBool("isSliding", true); // �����͹����ѹ��Ŵ�

        // ����¹��Ҵ��е��˹觢ͧ Collider �������Ŵ�
        boxCollider.size = slideColliderSize;
        boxCollider.offset = slideColliderOffset;

        // ����������Ŵ��¡�õ�駤�� velocity �����ȷҧ�����ѧ�Թ����
        rb.velocity = new Vector2(moveX * slideForce, rb.velocity.y);

        // ���ѡ�ѡ��͹��ش��Ŵ�������顴������ҧ
        yield return new WaitForSeconds(slideDuration);

        if (!playerInput.actions["Slide"].IsPressed())
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        isSliding = false;
        playeranim.SetBool("isSliding", false); // ��ش͹����ѹ��Ŵ�

        // �׹��Ҵ��е��˹觢ͧ Collider �繢�Ҵ����
        boxCollider.size = normalColliderSize;
        boxCollider.offset = normalColliderOffset;

        // ��ش�������͹�����ǹ͹��ѧ�����Ŵ�
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        if (!isSliding) // ��ͧ�ѹ����������͹�������������������ʶҹ���Ŵ�
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
        // ��ҡ��ѧ��Ŵ� ����ʴ�͹����ѹ��Ŵ����ҧ����
        playeranim.SetBool("isSliding", true);
        playeranim.SetBool("run", false);
    }
    else
    {
        // ����������Ŵ� �ʴ�͹����ѹ���������ش���ʶҹС������͹���
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
