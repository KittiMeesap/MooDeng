using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    public float magnetDuration = 3f; // ระยะเวลาที่แม่เหล็กทำงาน

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // เปิดใช้งานแม่เหล็กในตัวผู้เล่น
            MagnetItem magnetItem = collision.GetComponent<MagnetItem>();
            if (magnetItem != null)
            {
                magnetItem.ActivateMagnet(magnetDuration);
            }

            // ทำลายไอเท็มแม่เหล็ก
            Destroy(gameObject);
        }
    }
}
