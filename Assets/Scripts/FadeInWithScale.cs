using System.Collections;
using UnityEngine;

public class CustomizableFadeScaleRotate : MonoBehaviour
{
    public bool enableFade = true;            // เปิดหรือปิดการเฟด
    public bool enableScale = true;           // เปิดหรือปิดการขยายขนาด
    public bool enableRotate = false;         // เปิดหรือปิดการหมุน

    public float duration = 1f;               // ระยะเวลาการเฟดและขยายขนาด
    public Vector3 targetScale = Vector3.one; // ขนาดสุดท้ายที่ต้องการให้แสดง
    public float rotationSpeed = 1080f;       // ความเร็วในการหมุน (องศาต่อวินาที)

    private SpriteRenderer spriteRenderer;    // ใช้สำหรับควบคุมค่าความโปร่งแสง (alpha)

    private void OnEnable()
    {
        // ตั้งค่าเริ่มต้นขนาดและ alpha ของ SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enableScale)
        {
            transform.localScale = Vector3.zero; // ตั้งค่า scale เริ่มต้นเป็น 0 ถ้าต้องการเอฟเฟกต์ขยายขนาด
        }

        if (spriteRenderer != null && enableFade)
        {
            Color color = spriteRenderer.color;
            color.a = 0; // ตั้งค่า alpha เริ่มต้นเป็น 0 ถ้าต้องการเอฟเฟกต์เฟด
            spriteRenderer.color = color;
        }

        // เริ่ม Coroutine เพื่อให้การเฟด ขยายขนาด และหมุน (ถ้ากำหนดให้เปิด) เกิดขึ้น
        StartCoroutine(FadeScaleAndRotate(true));
    }

    // ฟังก์ชันที่ใช้เพื่อทำการ fade out และย่อขนาด จากนั้นค่อยปิดวัตถุ
    public void FadeOutAndDeactivate()
    {
        StartCoroutine(FadeScaleAndRotate(false, true));
    }

    private IEnumerator FadeScaleAndRotate(bool isShowing, bool deactivateOnEnd = false)
    {
        float elapsedTime = 0f;
        Vector3 startScale = isShowing ? Vector3.zero : targetScale;
        Vector3 endScale = isShowing ? targetScale : Vector3.zero;
        float startAlpha = isShowing ? 0f : 1f;
        float endAlpha = isShowing ? 1f : 0f;

        while (elapsedTime < duration)
        {
            // คำนวณเปอร์เซ็นต์ของการเปลี่ยนแปลงในแต่ละเฟรม
            float progress = elapsedTime / duration;

            // ปรับขนาด (ขยายหรือย่อ) ตามทิศทางที่กำหนด
            if (enableScale)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            }

            // ปรับค่า alpha (โปร่งแสง) ถ้าเปิดใช้งานการเฟด
            if (spriteRenderer != null && enableFade)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(startAlpha, endAlpha, progress);
                spriteRenderer.color = color;
            }

            // หมุนวัตถุถ้า enableRotate เป็น true
            if (enableRotate)
            {
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime * (isShowing ? 1 : -1));
            }

            // เพิ่มเวลาที่ผ่านไป
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ตั้งค่าขนาดและ alpha ให้เป็นค่าเป้าหมายเมื่อการเฟดและขยายเสร็จสิ้น
        if (enableScale)
        {
            transform.localScale = endScale;
        }

        if (spriteRenderer != null && enableFade)
        {
            Color color = spriteRenderer.color;
            color.a = endAlpha;
            spriteRenderer.color = color;
        }

        // ถ้าการหมุนเสร็จสิ้น ให้ตั้งค่า rotation กลับไปที่ (0, 0, 0)
        if (enableRotate)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // ปิดการแสดงผลของ GameObject ถ้ากำหนดให้ปิดเมื่อจบการทำงาน
        if (deactivateOnEnd)
        {
            gameObject.SetActive(false);
        }
    }
}
