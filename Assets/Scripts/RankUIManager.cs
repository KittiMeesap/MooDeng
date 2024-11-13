using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using SimpleJSON;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;


public class RankUIManager : MonoBehaviour
{
    public GameObject rankDataPrefab;
    public Transform rankPanel;
    public TMP_Dropdown levelDropdown; // Dropdown สำหรับเลือกด่าน

    public List<PlayerData> playerDatas = new List<PlayerData>();
    public List<GameObject> createdPlayerData = new List<GameObject>();

    public RankData yourRankData;

    private const string url = "https://moodengadventure-default-rtdb.asia-southeast1.firebasedatabase.app";
    private const string secret = "AIzaSyCa734MabQp-CNWa3wmJ5b8un6HyH21XzI";
    private bool isLoadingData = false;

    private void Start()
    {
        // โหลดชื่อและเวลาของผู้เล่นจาก PlayerPrefs
        string currentPlayerName = PlayerPrefs.GetString("CurrentPlayerName", "Unknown Player");
        float currentPlayerTime = PlayerPrefs.GetFloat("CurrentPlayerTime", 0f);

        // กำหนดข้อมูลผู้เล่นปัจจุบันใน yourRankData
        yourRankData.playerData = new PlayerData
        {
            playerName = currentPlayerName,
            playerTime = currentPlayerTime
        };

        // เคลียร์ข้อมูลใน rankPanel ก่อน
        ClearRankData();

        // เพิ่มตัวเลือกด่านใน dropdown
        PopulateDropdown();

        // ตั้งค่าเริ่มต้นของ Dropdown ตามด่านล่าสุดที่ผู้เล่นผ่าน
        SetDropdownToLastCompletedLevel();

        // โหลดข้อมูลอันดับของผู้เล่น
        levelDropdown.onValueChanged.AddListener(delegate {
            LoadLeaderboardData(levelDropdown.options[levelDropdown.value].text);
        });

        // โหลดข้อมูลอันดับเริ่มต้นของด่านล่าสุด
        LoadLeaderboardData(levelDropdown.options[levelDropdown.value].text);
    }

    public void OnGoNextLevelButtonClicked()
    {
        // ดึงชื่อของซีนจากตัวเลือกใน Dropdown
        string selectedLevel = levelDropdown.options[levelDropdown.value].text;

        // ตรวจสอบว่าชื่อของซีนอยู่ในรูปแบบ "LevelX" โดยที่ X เป็นตัวเลข
        if (selectedLevel.StartsWith("Level"))
        {
            // ดึงตัวเลขจากชื่อของซีนปัจจุบัน
            string levelNumberStr = selectedLevel.Substring(5); // เอาส่วนที่เป็นตัวเลขหลัง "Level"
            int levelNumber;

            // พยายามแปลงตัวเลขเป็น integer
            if (int.TryParse(levelNumberStr, out levelNumber))
            {
                // เพิ่ม 1 เพื่อสร้างชื่อซีนถัดไป
                int nextLevelNumber = levelNumber + 1;
                string nextLevelName = "Level" + nextLevelNumber;

                // โหลดซีนตามชื่อที่สร้าง
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogError("Failed to parse level number from selected level name.");
            }
        }
        else
        {
            Debug.LogError("Selected level name does not start with 'Level'.");
        }
    }
    private void CalculateRankForCurrentPlayer()
    {
        // สร้างรายการใหม่ที่เรียงลำดับ playerDatas ตาม playerTime โดยไม่เพิ่มผู้เล่นปัจจุบันเข้าไปในรายการ
        List<PlayerData> sortedPlayerDatas = playerDatas.OrderBy(data => data.playerTime).ToList();

        // หาลำดับของผู้เล่นปัจจุบันใน sortedPlayerDatas
        int rank = 1;
        foreach (PlayerData data in sortedPlayerDatas)
        {
            if (data.playerName == yourRankData.playerData.playerName)
            {
                yourRankData.playerData.rankNumber = rank;
                yourRankData.UpdateData(); // อัปเดตข้อมูลใน UI สำหรับผู้เล่นปัจจุบัน
                break;
            }
            rank++;
        }
    }





    public void FindYourDataInRanking()
    {
        // ค้นหาข้อมูลของผู้เล่นในรายการอันดับจาก playerDatas ที่มีอยู่ใน RankUIManager
        PlayerData currentPlayerData = playerDatas
            .Where(data => data.playerName == yourRankData.playerData.playerName)
            .FirstOrDefault();

        // อัปเดตข้อมูลใน yourRankData
        yourRankData.playerData = currentPlayerData;
        yourRankData.UpdateData();

        Debug.LogWarning("ข้อมูลของผู้เล่น: " + currentPlayerData.playerName);
    }

    private void PopulateDropdown()
    {
        // เพิ่มตัวเลือกด่านใน dropdown
        levelDropdown.options.Clear();
        levelDropdown.options.Add(new TMP_Dropdown.OptionData("Level1"));
        levelDropdown.options.Add(new TMP_Dropdown.OptionData("Level2"));
        levelDropdown.options.Add(new TMP_Dropdown.OptionData("Level3"));
        // เพิ่มด่านอื่นๆ ตามต้องการ
    }

    private void SetDropdownToLastCompletedLevel()
    {
        // ดึงข้อมูลของด่านล่าสุดที่ผู้เล่นผ่าน
        string lastCompletedLevel = PlayerPrefs.GetString("LastCompletedLevel", "Level1");

        // ค้นหา index ของด่านล่าสุดใน Dropdown
        int levelIndex = levelDropdown.options.FindIndex(option => option.text == lastCompletedLevel);
        if (levelIndex >= 0)
        {
            levelDropdown.value = levelIndex;
            levelDropdown.RefreshShownValue();
        }

        // โหลดข้อมูล leaderboard สำหรับด่านที่ตั้งค่าเริ่มต้นไว้
        LoadLeaderboardData(lastCompletedLevel);
    }

    public void LoadLeaderboardData(string levelID)
    {
        if (isLoadingData)
        {
            Debug.LogWarning("Data is currently loading, skipping duplicate load request.");
            return; // ข้ามถ้ากำลังโหลดอยู่แล้ว
        }

        isLoadingData = true; // ตั้งสถานะว่ากำลังโหลดข้อมูล
        ClearRankData(); // ล้างข้อมูลใน UI ก่อนโหลดใหม่
        playerDatas.Clear(); // ล้างข้อมูลใน playerDatas ก่อนโหลดข้อมูลใหม่

        string loadUrl = $"{url}/ranking/{levelID}/playerDatas.json?auth={secret}";

        Debug.Log($"Loading data from Firebase for level: {levelID}");

        RestClient.Get(loadUrl).Then(response =>
        {
            JSONNode jsonNode = JSONNode.Parse(response.Text);
            Debug.Log("Data loaded successfully from Firebase.");

            for (int i = 0; i < jsonNode.Count; i++)
            {
                PlayerData newData = new PlayerData(
                    rankNumber: i + 1,
                    playerName: jsonNode[i]["playerName"],
                    playerTime: jsonNode[i]["playerTime"],
                    profileSprite: null
                );

                playerDatas.Add(newData);
            }

            CalculateRankForCurrentPlayer();
            DisplayRankData();
        }).Catch(error =>
        {
            Debug.LogError("เกิดข้อผิดพลาดในการดึงข้อมูลจากเซิร์ฟเวอร์: " + error.Message);
        }).Finally(() =>
        {
            isLoadingData = false; // ปลดล็อกสถานะเมื่อโหลดเสร็จสิ้น
        });
    }




    private void DisplayRankData()
    {
        ClearRankData(); // ล้างข้อมูลเก่าออกจาก UI
        Debug.Log("Displaying rank data...");

        foreach (PlayerData playerData in playerDatas)
        {
            Debug.Log($"Displaying Player - Rank: {playerData.rankNumber}, Name: {playerData.playerName}, Time: {playerData.playerTime}");

            GameObject rankObj = Instantiate(rankDataPrefab, rankPanel);
            RankData rankData = rankObj.GetComponent<RankData>();
            rankData.playerData = playerData;
            rankData.UpdateData();
            createdPlayerData.Add(rankObj); // เก็บอ้างอิงของข้อมูลที่สร้าง
        }

        Debug.Log("Finished displaying rank data.");
    }




    private void ClearRankData()
    {
        Debug.Log("Clearing previous rank data...");

        foreach (GameObject createdData in createdPlayerData)
        {
            Destroy(createdData);
        }
        createdPlayerData.Clear(); // เคลียร์รายการที่เก็บอ้างอิง GameObject

        Debug.Log("Cleared all rank data.");
    }



    public void ReloadRankData()
    {
        ClearRankData();
        DisplayRankData();
    }

    public void AddPlayerData(string playerName, float playerTime, Sprite profileSprite)
    {
        PlayerData newPlayer = new PlayerData
        {
            playerName = playerName,
            playerTime = playerTime,
            profileSprite = profileSprite
        };

        playerDatas.Add(newPlayer);
        ReloadRankData();
    }
}
