using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public float powerUpDuration = 3f; // ระยะเวลาเพิ่มพลัง

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // เปิดสถานะเพิ่มพลัง
                player.ActivatePowerUp(powerUpDuration);

                // ทำลายไอเท็มเมื่อเก็บ
                Destroy(gameObject);
            }
        }
    }
}

