using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    public float minScale = 0.5f; // ขนาดย่อสุด
    public float maxScale = 1.5f; // ขนาดขยายสุด
    public float speed = 2.0f; // ความเร็วในการย่อขยาย (เพิ่มค่าความเร็วเพื่อให้ขยับถี่ขึ้น)

    void Update()
    {
        // คำนวณขนาดตามเวลาโดยใช้ Mathf.PingPong
        float scale = Mathf.PingPong(Time.time * speed, maxScale - minScale) + minScale;

        // ปรับขนาดของ GameObject ตามค่าที่คำนวณ
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
