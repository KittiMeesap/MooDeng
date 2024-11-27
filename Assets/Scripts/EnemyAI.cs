using UnityEngine;
using System.Collections;
using System;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // ����Фü�����
    public float detectionRadius = 10f; // ����շ���ѵ�ٵ�Ǩ�Ѻ������
    public float attackRadius = 2f; // ���з���ѵ�٨����������
    public float moveSpeed = 2f; // �������ǡ������͹���ͧ�ѵ��
    public float attackCooldown = 2f; // �����������ҧ����������Ф���

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private Vector2 startingPosition; // ���˹�������鹢ͧ�ѵ��
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position; // �ѹ�֡���˹�������鹢ͧ�ѵ��
}

    void Update()
    {
        // ��Ǩ�ͺ������ҧ�����ҧ�����蹡Ѻ�ѵ��
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // �ҡ��������е�Ǩ�Ѻ �������͹�����Ҽ�����
            if (distanceToPlayer > attackRadius)
            {
                MoveTowardsPlayer();
            }
            else if (!isAttacking)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController.IsPoweredUp())
                {
                    Destroy(gameObject);
                }
                else
                {
                    // �����������ʶҹ�������ѧ ����ѵ������
                    GameManager.instance.Death();
                }
                // �ҡ������������� �������
            }
        }
        else
        {
            // �ҡ����͡���е�Ǩ�Ѻ ����Ѻ仵��˹��������
            MoveToStartingPosition();
        }
    }

    private void MoveTowardsPlayer()
    {
        // ����͹�����ѧ���˹觼�����
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void MoveToStartingPosition()
    {
        // ����͹����Ѻ��ѧ���˹��������
        Vector2 direction = (startingPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // ��ش����Ͷ֧���˹��������
        if (Vector2.Distance(transform.position, startingPosition) < 0.1f)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // �ʴ�����ա�õ�Ǩ�Ѻ��С������� Scene View
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
