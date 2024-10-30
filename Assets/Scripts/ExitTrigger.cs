using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.CheckTotalCoins())
            {
                UnlockNewLevel();
                StartCoroutine(LevelExit());
            }
            else
            {
                Debug.Log("ยังเก็บเหรียญไม่ครบตามที่กำหนด");
            }
        }
    }

    IEnumerator LevelExit()
    {
        UIManager.instance.fadeToBlack = true;
        yield return new WaitForSeconds(0.5f);

        GameManager.instance.LevelComplete(); // บันทึกข้อมูลและจบเลเวล
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<GameManager>().OnPlayerFinish();
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
