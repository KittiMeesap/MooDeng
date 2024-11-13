using System.Collections;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public Transform exitPosition; // ตำแหน่งที่ Player จะถูกดูดเข้าไป

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.CheckTotalCoins())
            {
                UnlockNewLevel();
                StartCoroutine(LevelExit(collision.gameObject));
            }
            else
            {
                Debug.Log("ยังเก็บเหรียญไม่ครบตามที่กำหนด");
            }
        }
    }

    private IEnumerator LevelExit(GameObject player)
    {
        // เรียกใช้ FadeOutAndDeactivate เพื่อให้ Player ค่อยๆ หมุนและย่อขนาด
        CustomizableFadeScaleRotate fadeScaleRotate = player.GetComponent<CustomizableFadeScaleRotate>();
        fadeScaleRotate.FadeOutAndDeactivate();

        float duration = fadeScaleRotate.duration; // ใช้ระยะเวลาเดียวกับการ Fade Out
        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = exitPosition.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            // เคลื่อนที่ Player ไปยังตำแหน่งของ exitPosition พร้อมกับย่อขนาด
            player.transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ตั้งค่าตำแหน่งสุดท้ายเมื่อ Player หายไป
        player.transform.position = endPosition;

        // เรียกใช้งาน LevelComplete หลังจาก Player หายไป
        GameManager.instance.LevelComplete();
    }

    private void UnlockNewLevel()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
