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
    public TMP_Dropdown levelDropdown; // Dropdown ����Ѻ���͡��ҹ

    public List<PlayerData> playerDatas = new List<PlayerData>();
    public List<GameObject> createdPlayerData = new List<GameObject>();

    public RankData yourRankData;

    private const string url = "https://moodengadventure-default-rtdb.asia-southeast1.firebasedatabase.app";
    private const string secret = "AIzaSyCa734MabQp-CNWa3wmJ5b8un6HyH21XzI";

    private void Start()
    {
        // ����������ŷ������ rankPanel ��͹
        ClearRankData();

        // ����������͡��ҹ� dropdown
        PopulateDropdown();

        // ��駤��������鹢ͧ Dropdown �����ҹ����ش�������蹼�ҹ
        SetDropdownToLastCompletedLevel();

        // ���� listener ����Ѻ�������¹�ŧ�ͧ dropdown
        levelDropdown.onValueChanged.AddListener(delegate {
            LoadLeaderboardData(levelDropdown.options[levelDropdown.value].text);
        });
    }


    private void PopulateDropdown()
    {
        // ����������͡��ҹ� dropdown
        levelDropdown.options.Clear();
        levelDropdown.options.Add(new TMP_Dropdown.OptionData("Level1"));
        levelDropdown.options.Add(new TMP_Dropdown.OptionData("Level2"));
        levelDropdown.options.Add(new TMP_Dropdown.OptionData("Level3"));
        // ������ҹ���� �����ͧ���
    }

    private void SetDropdownToLastCompletedLevel()
    {
        // �֧�����Ţͧ��ҹ����ش�������蹼�ҹ
        string lastCompletedLevel = PlayerPrefs.GetString("LastCompletedLevel", "Level1");

        // ���� index �ͧ��ҹ����ش� Dropdown
        int levelIndex = levelDropdown.options.FindIndex(option => option.text == lastCompletedLevel);
        if (levelIndex >= 0)
        {
            levelDropdown.value = levelIndex;
            levelDropdown.RefreshShownValue();
        }

        // ��Ŵ������ leaderboard ����Ѻ��ҹ����駤������������
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
                    profileSprite: null // ����� null �ҡ������ٻ�Ҿ
                );

                Debug.Log($"Adding player data - Rank: {newData.rankNumber}, Name: {newData.playerName}, Time: {newData.playerTime}");
                playerDatas.Add(newData);
            }

            DisplayRankData();
        }).Catch(error =>
        {
            Debug.Log("�Դ��ͼԴ��Ҵ㹡�ô֧�����Ũҡ���������: " + error.Message);
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
            createdPlayerData.Add(rankObj); // ����ҧ�ԧ�ͧ�����ŷ�����ҧ
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
