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
    [SerializeField] private TMP_Text timerText; // Text ����Ѻ�ʴ����ҷ����㹴�ҹ
    [SerializeField] private PlayerController playerController;

    private int coinCount = 0;
    private bool isGameOver = false;
    private Vector3 playerPosition;

    private float timer = 0f; // ��ǹѺ���ҷ����㹴�ҹ
    private bool isTiming = true; // �Ǻ�����ùѺ����

    // Level Complete UI
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TMP_Text levelCompleteText;
    [SerializeField] private TMP_Text levelCompleteCoins;
    [SerializeField] private TMP_Text levelCompleteTime; // Text ����Ѻ�ʴ����Ҩ��˹�Ҩ� Level Complete
    [SerializeField] private GameObject leaderboardButton;
    [SerializeField] private GameObject hideui;
    [SerializeField] private GameObject portal;
    [SerializeField] private GameObject player;

    private int totalCoins = 0; // �ӹǹ����­����ͧ������д�ҹ
    [SerializeField] private string playerName = "Player"; // ���ͼ�����
    [SerializeField] private string levelID = "Level1"; // ID �ͧ��ҹ�Ѩ�غѹ

    private const string url = "https://moodengadventure-default-rtdb.asia-southeast1.firebasedatabase.app"; // URL �ͧ Firebase
    private const string secret = "AIzaSyCa734MabQp-CNWa3wmJ5b8un6HyH21XzI"; // Secret Key �ͧ Firebase

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
            timer += Time.deltaTime; // ������Ңͧ timer ��������
            UpdateTimerUI(); // �ѻവ����ʴ��Ţͧ����� UI
        }

        if (coinCount >= totalCoins)
        {
            portal.SetActive(true);
        }
    }

    private void UpdateTimerUI()
    {
        // �֧ 5 ����á�ͧ levelID
        string firstPart = levelID.Length >= 5 ? levelID.Substring(0, 5) : levelID;

        // �֧����ѡ�÷������ͷ�������ѧ�ҡ��Ƿ�� 5
        string lastPart = levelID.Length > 5 ? levelID.Substring(5) : "";

        // �����ШѴ�ٻẺ
        string formattedLevelID = $"{firstPart} {lastPart}";

        // �ʴ����� formattedLevelID ���������ٻẺ�ȹ���
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
            isTiming = false; // ��ش��ùѺ�����ҡ���������
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
        return coinCount >= totalCoins; // ��Ǩ�ͺ��Ҽ�����������­�ú�������˹��������
    }

    public void LevelComplete()
    {
        isTiming = false;

        // �Ѵ�ٻẺ LevelID
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

            // ��˹������ż����蹻Ѩ�غѹ
            currentPlayerData = new PlayerData
            {
                playerName = playerName,
                playerTime = timer // ���ҷ����㹴�ҹ
            };

            // ��Ǩ�ͺ����բ����Ţͧ������� ranking �����������
            PlayerData checkPlayerData = ranking.playerDatas.FirstOrDefault(data => data.playerName == currentPlayerData.playerName);
            int indexOfPlayer = ranking.playerDatas.IndexOf(checkPlayerData);

            if (checkPlayerData.playerName != null)
            {
                // �ѻവ���Ңͧ������� ranking
                checkPlayerData.playerTime = currentPlayerData.playerTime;
                ranking.playerDatas[indexOfPlayer] = checkPlayerData;
            }
            else
            {
                // ��������������� ranking
                ranking.playerDatas.Add(currentPlayerData);
            }

            // �Ѵ�ѹ�Ѻ�������
            CalculateRankFromScore();

            // �ѹ�֡������ ranking ��������Ѻ价�� Firebase
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
        // �Ѵ���§������� ranking ������� ����ѻവ�ѹ�Ѻ
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
        SceneManager.LoadScene("Leaderboard"); // ����¹���ͫչ���������������Ѻ˹�� Leaderboard
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
        isTiming = false; // ��ش��ùѺ�����ҡ�����蹨������
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
        public Sprite profileSprite; // �ҡ������� ����ö�Ѵ�͡��
    }

    [System.Serializable]
    public class Ranking
    {
        public List<PlayerData> playerDatas;
    }
}

