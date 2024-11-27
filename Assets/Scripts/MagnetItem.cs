using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    public float magnetRadius = 5f; // รัศมีแม่เหล็ก
    public float magnetSpeed = 10f; // ความเร็วในการดูดไอเท็ม
    public string itemTag = "Coin"; // Tag ของไอเท็มที่ต้องการดูด

    private bool isMagnetActive = false; // สถานะแม่เหล็ก
    private SpriteRenderer spriteRenderer; // ใช้สำหรับเปลี่ยนสี Player
    private Color originalColor; // เก็บสีเดิมของ Player

    void Start()
    {
        // ค้นหา SpriteRenderer ใน Object ลูก
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // บันทึกสีเดิมของ Player
        }
        else
        {
            Debug.LogError("SpriteRenderer not found in child objects!");
        }
    }

    void Update()
    {
        if (isMagnetActive)
        {
            // ค้นหาไอเท็มในรัศมีที่มี Tag ตรงกัน
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            foreach (Collider2D item in items)
            {
                if (item.CompareTag(itemTag))
                {
                    // ดึงไอเท็มเข้าหาผู้เล่น
                    Vector2 direction = (transform.position - item.transform.position).normalized;
                    item.transform.position += (Vector3)(direction * magnetSpeed * Time.deltaTime);

                    // เมื่อไอเท็มชนตัวผู้เล่น
                    if (Vector2.Distance(item.transform.position, transform.position) < 0.5f)
                    {
                        Debug.Log($"Item {item.name} collected!");
                        Destroy(item.gameObject); // ทำลายไอเท็ม
                    }
                }
            }
        }
    }

    public void ActivateMagnet(float duration)
    {
        // เปิดใช้งานแม่เหล็ก
        isMagnetActive = true;
        Debug.Log("Magnet activated!");

        // เปลี่ยนสี Player เป็นสีม่วง
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.magenta;
        }

        // ปิดแม่เหล็กหลังจากเวลาหมด
        Invoke(nameof(DeactivateMagnet), duration);
    }

    private void DeactivateMagnet()
    {
        // ปิดการใช้งานแม่เหล็ก
        isMagnetActive = false;

        // คืนสี Player เป็นสีเดิม
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        Debug.Log("Magnet deactivated.");
    }

    private void OnDrawGizmosSelected()
    {
        // แสดงรัศมีแม่เหล็กใน Scene View
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
