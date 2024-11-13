using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Proyecto26;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText; // Text สำหรับแสดงเวลาที่ใช้ในด่าน
    [SerializeField] private PlayerController playerController;

    private int coinCount = 0;
    private bool isGameOver = false;
    private Vector3 playerPosition;

    private float timer = 0f; // ตัวนับเวลาที่ใช้ในด่าน
    private bool isTiming = true; // ควบคุมการนับเวลา

    // Level Complete UI
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TMP_Text levelCompleteText;
    [SerializeField] private TMP_Text levelCompleteCoins;
    [SerializeField] private TMP_Text levelCompleteTime; // Text สำหรับแสดงเวลาจบในหน้าจอ Level Complete
    [SerializeField] private GameObject leaderboardButton;
    [SerializeField] private GameObject hideui;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject player;

    private int totalCoins = 0; // จำนวนเหรียญที่ต้องเก็บในแต่ละด่าน
    [SerializeField] private string playerName = "Player"; // ชื่อผู้เล่น
    [SerializeField] private string levelID = "Level1"; // ID ของด่านปัจจุบัน

    private const string url = "https://moodengadventure-default-rtdb.asia-southeast1.firebasedatabase.app"; // URL ของ Firebase
    private const string secret = "AIzaSyCa734MabQp-CNWa3wmJ5b8un6HyH21XzI"; // Secret Key ของ Firebase

    [HideInInspector] Ranking ranking = new Ranking();
    [HideInInspector] PlayerData currentPlayerData;

    public GameObject GetPlayer()
    {
        return player;
    }
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        portal.SetActive(false);
        FindTotalPickups();
        UpdateGUI();
    }

    private void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime; // เพิ่มค่าของ timer ในแต่ละเฟรม
            UpdateTimerUI(); // อัปเดตการแสดงผลของเวลาใน UI
        }

        if (coinCount >= totalCoins)
        {
            portal.SetActive(true);
        }
    }

    private void UpdateTimerUI()
    {
        // ดึง 5 ตัวแรกของ levelID
        string firstPart = levelID.Length >= 5 ? levelID.Substring(0, 5) : levelID;

        // ดึงตัวอักษรที่เหลือทั้งหมดหลังจากตัวที่ 5
        string lastPart = levelID.Length > 5 ? levelID.Substring(5) : "";

        // รวมและจัดรูปแบบ
        string formattedLevelID = $"{firstPart} {lastPart}";

        // แสดงผลเป็น formattedLevelID และเวลาในรูปแบบทศนิยม
        timerText.text = $"{formattedLevelID}\n{timer:F2}";
    }

    public void IncrementCoinCount()
    {
        coinCount++;
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        coinText.text = $"{coinCount} / {totalCoins}";
    }

    public void Death()
    {
        if (!isGameOver)
        {
            playerController.gameObject.SetActive(false);
            StartCoroutine(DeathCoroutine());
            isGameOver = true;
            isTiming = false; // หยุดการนับเวลาหากเกมโอเวอร์
        }
    }

    public void FindTotalPickups()
    {
        pickup[] pickups = GameObject.FindObjectsOfType<pickup>();
        foreach (pickup pickupObject in pickups)
        {
            if (pickupObject.pt == pickup.pickupType.coin)
            {
                totalCoins += 1;
            }
        }
    }

    public bool CheckTotalCoins()
    {
        return coinCount >= totalCoins; // ตรวจสอบว่าผู้เล่นเก็บเหรียญครบตามที่กำหนดหรือไม่
    }

    public void LevelComplete()
    {
        isTiming = false;

        // จัดรูปแบบ LevelID
        string firstPart = levelID.Length >= 5 ? levelID.Substring(0, 5) : levelID;
        string lastPart = levelID.Length > 5 ? levelID.Substring(5) : "";
        string formattedLevelID = $"{firstPart} {lastPart}";

        levelCompletePanel.SetActive(true);
        levelCompleteText.text = $"{firstPart} {lastPart} Complete";

        levelCompleteCoins.text = $"Carrot Collected : {coinCount} / {totalCoins}";
        levelCompleteTime.text = $"Time : {timer:F2} s";

        leaderboardButton.SetActive(true);
        hideui.SetActive(false);
        player.SetActive(false);

        SavePlayerScoreToDatabase();

        PlayerPrefs.SetString("LastCompletedLevel", levelID);
        PlayerPrefs.SetString("CurrentPlayerName", playerName);
        PlayerPrefs.SetFloat("CurrentPlayerTime", timer);
        PlayerPrefs.Save();
    }


    public void SavePlayerScoreToDatabase()
    {
        string urlData = $"{url}/ranking/{levelID}/playerDatas.json?auth={secret}";

        RestClient.Get(urlData).Then(response =>
        {
            Debug.Log(response.Text);
            JSONNode jsonNode = JSONNode.Parse(response.Text);
            ranking.playerDatas = new List<PlayerData>();
            for (int i = 0; i < jsonNode.Count; i++)
            {
                ranking.playerDatas.Add(new PlayerData(
                    rankNumber: jsonNode[i]["rankNumber"],
                    playerName: jsonNode[i]["playerName"],
                    playerTime: jsonNode[i]["playerTime"],
                    profileSprite: null));
            }

            // กำหนดข้อมูลผู้เล่นปัจจุบัน
            currentPlayerData = new PlayerData
            {
                playerName = playerName,
                playerTime = timer // เวลาที่ใช้ในด่าน
            };

            // ตรวจสอบว่ามีข้อมูลของผู้เล่นใน ranking อยู่หรือไม่
            PlayerData checkPlayerData = ranking.playerDatas.FirstOrDefault(data => data.playerName == currentPlayerData.playerName);
            int indexOfPlayer = ranking.playerDatas.IndexOf(checkPlayerData);

            if (checkPlayerData.playerName != null)
            {
                // อัปเดตเวลาของผู้เล่นใน ranking
                checkPlayerData.playerTime = currentPlayerData.playerTime;
                ranking.playerDatas[indexOfPlayer] = checkPlayerData;
            }
            else
            {
                // เพิ่มผู้เล่นใหม่ใน ranking
                ranking.playerDatas.Add(currentPlayerData);
            }

            // จัดอันดับตามเวลา
            CalculateRankFromScore();

            // บันทึกข้อมูล ranking ทั้งหมดกลับไปที่ Firebase
            string urlPlayerData = $"{url}/ranking/{levelID}.json?auth={secret}";

            RestClient.Put(urlPlayerData, ranking).Then(response =>
            {
                Debug.Log("Upload Data Complete");
            }).Catch(error =>
            {
                Debug.Log("Error on set to server: " + error.Message);
            });
        }).Catch(error =>
        {
            Debug.Log("Error to get data from server: " + error.Message);
        });
    }

    private void CalculateRankFromScore()
    {
        // จัดเรียงข้อมูลใน ranking ตามเวลา และอัปเดตอันดับ
        List<PlayerData> sortRankPlayers = ranking.playerDatas.OrderBy(data => data.playerTime).ToList();

        for (int i = 0; i < sortRankPlayers.Count; i++)
        {
            PlayerData changedRankNum = sortRankPlayers[i];
            changedRankNum.rankNumber = i + 1;
            sortRankPlayers[i] = changedRankNum;
        }

        ranking.playerDatas = sortRankPlayers;
    }

    public void GoToLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard"); // เปลี่ยนชื่อซีนตามที่ตั้งไว้สำหรับหน้า Leaderboard
    }

    public IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);
        playerController.transform.position = playerPosition;

        yield return new WaitForSeconds(1f);

        if (isGameOver)
        {
            SceneManager.LoadScene(levelID);
        }
    }

    public void OnPlayerFinish()
    {
        isTiming = false; // หยุดการนับเวลาหากผู้เล่นจบเลเวล
    }
}

namespace GameNamespace
{
    [System.Serializable]
    public class PlayerData
    {
        public int rankNumber;
        public string playerName;
        public float playerTime;
        public Sprite profileSprite; // หากไม่ได้ใช้ สามารถตัดออกได้
    }

    [System.Serializable]
    public class Ranking
    {
        public List<PlayerData> playerDatas;
    }
}

