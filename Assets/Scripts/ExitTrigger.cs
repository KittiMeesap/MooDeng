using System.Collections;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public Transform exitPosition; // ���˹觷�� Player �ж١�ٴ����

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
                Debug.Log("�ѧ������­���ú�������˹�");
            }
        }
    }

    private IEnumerator LevelExit(GameObject player)
    {
        // ���¡�� FadeOutAndDeactivate ������� Player ����� ��ع�����͢�Ҵ
        CustomizableFadeScaleRotate fadeScaleRotate = player.GetComponent<CustomizableFadeScaleRotate>();
        fadeScaleRotate.FadeOutAndDeactivate();

        float duration = fadeScaleRotate.duration; // �������������ǡѺ��� Fade Out
        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = exitPosition.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;

            // ����͹��� Player ��ѧ���˹觢ͧ exitPosition ������Ѻ��͢�Ҵ
            player.transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��駤�ҵ��˹��ش��������� Player ����
        player.transform.position = endPosition;

        // ���¡��ҹ LevelComplete ��ѧ�ҡ Player ����
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
