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
    public TMP_Dropdown levelDropdown; // Dropdown ����Ѻ���͡��ҹ

    public List<PlayerData> playerDatas = new List<PlayerData>();
    public List<GameObject> createdPlayerData = new List<GameObject>();

    public RankData yourRankData;

    private const string url = "https://moodengadventure-default-rtdb.asia-southeast1.firebasedatabase.app";
    private const string secret = "AIzaSyCa734MabQp-CNWa3wmJ5b8un6HyH21XzI";
    private bool isLoadingData = false;

    private void Start()
    {
        // ��Ŵ����������Ңͧ�����蹨ҡ PlayerPrefs
        string currentPlayerName = PlayerPrefs.GetString("CurrentPlayerName", "Unknown Player");
        float currentPlayerTime = PlayerPrefs.GetFloat("CurrentPlayerTime", 0f);

        // ��˹������ż����蹻Ѩ�غѹ� yourRankData
        yourRankData.playerData = new PlayerData
        {
            playerName = currentPlayerName,
            playerTime = currentPlayerTime
        };

        // ������������ rankPanel ��͹
        ClearRankData();

        // ����������͡��ҹ� dropdown
        PopulateDropdown();

        // ��駤��������鹢ͧ Dropdown �����ҹ����ش�������蹼�ҹ
        SetDropdownToLastCompletedLevel();

        // ��Ŵ�������ѹ�Ѻ�ͧ������
        levelDropdown.onValueChanged.AddListener(delegate {
            LoadLeaderboardData(levelDropdown.options[levelDropdown.value].text);
        });

        // ��Ŵ�������ѹ�Ѻ������鹢ͧ��ҹ����ش
        LoadLeaderboardData(levelDropdown.options[levelDropdown.value].text);
    }

    public void OnGoNextLevelButtonClicked()
    {
        // �֧���ͧ͢�չ�ҡ������͡� Dropdown
        string selectedLevel = levelDropdown.options[levelDropdown.value].text;

        // ��Ǩ�ͺ��Ҫ��ͧ͢�չ������ٻẺ "LevelX" �·�� X �繵���Ţ
        if (selectedLevel.StartsWith("Level"))
        {
            // �֧����Ţ�ҡ���ͧ͢�չ�Ѩ�غѹ
            string levelNumberStr = selectedLevel.Substring(5); // �����ǹ����繵���Ţ��ѧ "Level"
            int levelNumber;

            // �������ŧ����Ţ�� integer
            if (int.TryParse(levelNumberStr, out levelNumber))
            {
                // ���� 1 �������ҧ���ͫչ�Ѵ�
                int nextLevelNumber = levelNumber + 1;
                string nextLevelName = "Level" + nextLevelNumber;

                // ��Ŵ�չ������ͷ�����ҧ
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
        // ���ҧ��¡�����������§�ӴѺ playerDatas ��� playerTime ��������������蹻Ѩ�غѹ�������¡��
        List<PlayerData> sortedPlayerDatas = playerDatas.OrderBy(data => data.playerTime).ToList();

        // ���ӴѺ�ͧ�����蹻Ѩ�غѹ� sortedPlayerDatas
        int rank = 1;
        foreach (PlayerData data in sortedPlayerDatas)
        {
            if (data.playerName == yourRankData.playerData.playerName)
            {
                yourRankData.playerData.rankNumber = rank;
                yourRankData.UpdateData(); // �ѻവ������� UI ����Ѻ�����蹻Ѩ�غѹ
                break;
            }
            rank++;
        }
    }





    public void FindYourDataInRanking()
    {
        // ���Ң����Ţͧ���������¡���ѹ�Ѻ�ҡ playerDatas ���������� RankUIManager
        PlayerData currentPlayerData = playerDatas
            .Where(data => data.playerName == yourRankData.playerData.playerName)
            .FirstOrDefault();

        // �ѻവ������� yourRankData
        yourRankData.playerData = currentPlayerData;
        yourRankData.UpdateData();

        Debug.LogWarning("�����Ţͧ������: " + currentPlayerData.playerName);
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
        if (isLoadingData)
        {
            Debug.LogWarning("Data is currently loading, skipping duplicate load request.");
            return; // ������ҡ��ѧ��Ŵ��������
        }

        isLoadingData = true; // ���ʶҹ���ҡ��ѧ��Ŵ������
        ClearRankData(); // ��ҧ������� UI ��͹��Ŵ����
        playerDatas.Clear(); // ��ҧ������� playerDatas ��͹��Ŵ����������

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
            Debug.LogError("�Դ��ͼԴ��Ҵ㹡�ô֧�����Ũҡ���������: " + error.Message);
        }).Finally(() =>
        {
            isLoadingData = false; // �Ŵ��͡ʶҹ��������Ŵ�������
        });
    }




    private void DisplayRankData()
    {
        ClearRankData(); // ��ҧ����������͡�ҡ UI
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
        createdPlayerData.Clear(); // ��������¡�÷������ҧ�ԧ GameObject

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
