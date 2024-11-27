using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public float powerUpDuration = 3f; // ��������������ѧ

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // �Դʶҹ�������ѧ
                player.ActivatePowerUp(powerUpDuration);

                // �����������������
                Destroy(gameObject);
            }
        }
    }
}

