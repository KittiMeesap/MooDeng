using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // ตัวละครผู้เล่น
    public float detectionRadius = 10f; // รัศมีที่ศัตรูตรวจจับผู้เล่น
    public float attackRadius = 2f; // ระยะที่ศัตรูจะเริ่มโจมตี
    public float moveSpeed = 2f; // ความเร็วการเคลื่อนที่ของศัตรู
    public float attackCooldown = 2f; // เวลารอระหว่างการโจมตีแต่ละครั้ง

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private Vector2 startingPosition; // ตำแหน่งเริ่มต้นของศัตรู

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position; // บันทึกตำแหน่งเริ่มต้นของศัตรู
    }

    void Update()
    {
        // ตรวจสอบระยะห่างระหว่างผู้เล่นกับศัตรู
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // หากอยู่ในระยะตรวจจับ ให้เคลื่อนที่ไปหาผู้เล่น
            if (distanceToPlayer > attackRadius)
            {
                MoveTowardsPlayer();
            }
            else if (!isAttacking)
            {
                // หากอยู่ในระยะโจมตี ให้โจมตี
                GameManager.instance.Death();
            }
        }
        else
        {
            // หากอยู่นอกระยะตรวจจับ ให้กลับไปตำแหน่งเริ่มต้น
            MoveToStartingPosition();
        }
    }

    private void MoveTowardsPlayer()
    {
        // เคลื่อนที่ไปยังตำแหน่งผู้เล่น
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void MoveToStartingPosition()
    {
        // เคลื่อนที่กลับไปยังตำแหน่งเริ่มต้น
        Vector2 direction = (startingPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // หยุดเมื่อถึงตำแหน่งเริ่มต้น
        if (Vector2.Distance(transform.position, startingPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        Debug.Log("Enemy attacks the player!");
        

        // รอเวลาตาม cooldown ก่อนจะโจมตีอีกครั้ง
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        // แสดงรัศมีการตรวจจับและการโจมตีใน Scene View
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
