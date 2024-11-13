using System.Collections;
using UnityEngine;

public class UIFadeIn : MonoBehaviour
{
    public float fadeDuration = 1f;  // ระยะเวลาที่ใช้ในการเฟด
    private CanvasGroup canvasGroup; // CanvasGroup ที่ควบคุมการเฟด

    private void OnEnable()
    {
        // ดึง CanvasGroup มาจาก GameObject
        canvasGroup = GetComponent<CanvasGroup>();

        // ตรวจสอบว่า CanvasGroup มีอยู่หรือไม่
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0; // ตั้งค่าเริ่มต้นให้โปร่งใส
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogWarning("ไม่มี CanvasGroup ใน GameObject นี้");
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // คำนวณค่า alpha ตามเวลาที่ผ่านไป
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);

            // เพิ่มเวลาที่ผ่านไป
            elapsedTime += Time.deltaTime;

            yield return null; // รอไปจนกว่าเฟรมถัดไป
        }

        // ตั้งค่า alpha เป็น 1 เมื่อการเฟดเสร็จสมบูรณ์
        canvasGroup.alpha = 1;
    }
}
