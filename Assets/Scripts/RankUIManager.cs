using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using SimpleJSON;
using System.Linq;
using TMPro;

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

    private void Start()
    {
        // เคลียร์ข้อมูลทั้งหมดใน rankPanel ก่อน
        ClearRankData();

        // เพิ่มตัวเลือกด่านใน dropdown
        PopulateDropdown();

        // ตั้งค่าเริ่มต้นของ Dropdown ตามด่านล่าสุดที่ผู้เล่นผ่าน
        SetDropdownToLastCompletedLevel();

        // เพิ่ม listener สำหรับการเปลี่ยนแปลงของ dropdown
        levelDropdown.onValueChanged.AddListener(delegate {
            LoadLeaderboardData(levelDropdown.options[levelDropdown.value].text);
        });
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
        ClearRankData();

        string loadUrl = $"{url}/ranking/{levelID}/playerDatas.json?auth={secret}";

        RestClient.Get(loadUrl).Then(response =>
        {
            JSONNode jsonNode = JSONNode.Parse(response.Text);
            playerDatas.Clear();

            Debug.Log("Loaded data from Firebase for level: " + levelID);

            for (int i = 0; i < jsonNode.Count; i++)
            {
                PlayerData newData = new PlayerData(
                    rankNumber: i + 1,
                    playerName: jsonNode[i]["playerName"],
                    playerTime: jsonNode[i]["playerTime"],
                    profileSprite: null // ตั้งเป็น null หากไม่มีรูปภาพ
                );

                Debug.Log($"Adding player data - Rank: {newData.rankNumber}, Name: {newData.playerName}, Time: {newData.playerTime}");
                playerDatas.Add(newData);
            }

            DisplayRankData();
        }).Catch(error =>
        {
            Debug.Log("เกิดข้อผิดพลาดในการดึงข้อมูลจากเซิร์ฟเวอร์: " + error.Message);
        });
    }

    private void DisplayRankData()
    {
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
        createdPlayerData.Clear();

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
