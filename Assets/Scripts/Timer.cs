using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] RankUIManager rankUIManager;
    [SerializeField] string playerName = "Player";
    [SerializeField] Sprite playerProfile;

    float elapsedTime;
    bool isRunning = true;

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        rankUIManager.AddPlayerData(playerName, elapsedTime, playerProfile);
    }
}