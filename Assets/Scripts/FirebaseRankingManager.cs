using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using SimpleJSON;
using System.Linq;

[System.Serializable]
public struct Ranking
{
    public List<PlayerData> playerDatas;
}

public class FirebaseRankingManager : MonoBehaviour
{
    public static FirebaseRankingManager instance;

    public const string url = "https://moodengadventure-default-rtdb.asia-southeast1.firebasedatabase.app";
    public const string secret = "AIzaSyCa734MabQp-CNWa3wmJ5b8un6HyH21XzI";

    [Header("Main")]
    public RankUIManager rankUIManager;
    public Ranking ranking;

    [Header("New Data")]
    public PlayerData currentPlayerData;
    private List<PlayerData> sortPlayerDatas = new List<PlayerData>();

    private void Awake()
    {
        instance = this;
    }

    #region บันทึกคะแนนของผู้เล่นตามด่าน

    public void SavePlayerScore(string playerName, float playerTime, string levelID)
    {
        PlayerData newPlayerData = new PlayerData
        {
            playerName = playerName,
            playerTime = playerTime
        };

        // ระบุ URL ให้บันทึกตาม levelID
        string saveUrl = $"{url}/ranking/{levelID}/playerDatas.json?auth={secret}";
        RestClient.Put(saveUrl, newPlayerData).Then(response =>
        {
            Debug.Log($"บันทึกข้อมูลของผู้เล่นสำเร็จสำหรับด่าน {levelID}");
        }).Catch(error =>
        {
            Debug.Log("เกิดข้อผิดพลาดในการบันทึกข้อมูลของผู้เล่น: " + error.Message);
        });
    }

    #endregion

    #region ฟังก์ชันทดสอบการบันทึกและดึงข้อมูล

    [Header("Test")]
    public int testNum;
    [System.Serializable]
    public struct TestData
    {
        public int num;
        public string name;
    }
    [System.Serializable]
    public struct TestObjectData
    {
        public string name;
        public TestData testData;
    }

    public TestData testData = new TestData();

    public TestObjectData testObjectData = new TestObjectData();

    public void TestSetData()
    {
        string urlData = $"{url}/TestData.json?auth={secret}";

        testData.name = "AAA";
        testData.num = 1;

        RestClient.Put<TestData>(urlData, testData).Then(response =>
        {
            Debug.Log("Upload Data Complete");
        }).Catch(error =>
        {
            Debug.Log("Error on set to server");
            Debug.Log(error.Message);
        });
    }

    public void TestSetData2()
    {
        string urlData = $"{url}/TestObjectData.json?auth={secret}";

        testObjectData.testData.name = "BBB";
        testObjectData.testData.num = 2;

        RestClient.Put<TestObjectData>(urlData, testData).Then(response =>
        {
            Debug.Log("Upload Data Complete");
        }).Catch(error =>
        {
            Debug.Log("Error on set to server");
            Debug.Log(error.Message);
        });
    }

    public void TestGetData()
    {
        string urlData = $"{url}/TestData.json?auth={secret}";

        RestClient.Get(urlData).Then(response =>
        {
            Debug.Log(response);
            JSONNode jsonNode = JSONNode.Parse(response.Text);
            testNum = jsonNode["num"];
        }).Catch(error =>
        {
            Debug.Log("Error");
        });
    }

    public void TestGetData2()
    {
        string urlData = $"{url}/TestObjectData.json?auth={secret}";

        RestClient.Get(urlData).Then(response =>
        {
            Debug.Log(response);
            JSONNode jsonNode = JSONNode.Parse(response.Text);
            testNum = jsonNode["TestData"]["num"];
        }).Catch(error =>
        {
            Debug.Log("Error");
        });
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    [ContextMenu("Set Local Data To Database")]
    public void SetLocalDataToDatabase(string levelID)
    {
        string urlData = $"{url}/ranking/{levelID}.json?auth={secret}";
        RestClient.Put<Ranking>(urlData, ranking).Then(response =>
        {
            Debug.Log("Upload Data Complete");
        }).Catch(error =>
        {
            Debug.Log("Error to Set Ranking Data Server");
        });
    }

    public void CalculateRankFromScore()
    {
        List<PlayerData> sortRankPlayers = ranking.playerDatas.OrderBy(data => data.playerTime).ToList();

        for (int i = 0; i < sortRankPlayers.Count; i++)
        {
            PlayerData changedRankNum = sortRankPlayers[i];
            changedRankNum.rankNumber = i + 1;

            sortRankPlayers[i] = changedRankNum;
        }

        ranking.playerDatas = sortRankPlayers;
    }

    public void FindYourDataInRanking()
    {
        currentPlayerData = ranking.playerDatas
            .Where(data => data.playerName == currentPlayerData.playerName).FirstOrDefault();

        rankUIManager.yourRankData.playerData = currentPlayerData;

        rankUIManager.yourRankData.UpdateData();
        Debug.LogWarning("test" + currentPlayerData.playerName);
    }

    public void ReloadSortingData(string levelID)
    {
        string urlData = $"{url}/ranking/{levelID}/playerDatas.json?auth={secret}";

        RestClient.Get(urlData).Then(response =>
        {
            Debug.Log(response.Text);
            JSONNode jsonNode = JSONNode.Parse(response.Text);

            ranking = new Ranking();
            ranking.playerDatas = new List<PlayerData>();
            for (int i = 0; i < jsonNode.Count; i++)
            {
                ranking.playerDatas.Add(new PlayerData(
                    jsonNode[i]["rankNumber"],
                    jsonNode[i]["playerName"],
                    jsonNode[i]["playerTime"],
                    null));
            }
            CalculateRankFromScore();

            string urlPlayerData = $"{url}/ranking/{levelID}.json?auth={secret}";

            RestClient.Put<Ranking>(urlPlayerData, ranking).Then(response =>
            {
                Debug.Log("Upload Data Complete");
                rankUIManager.playerDatas = ranking.playerDatas;
                rankUIManager.ReloadRankData();
                FindYourDataInRanking();
            }).Catch(error =>
            {
                Debug.Log("Error on set to server");
            });
        }).Catch(error =>
        {
            Debug.Log("Error to get data from server");
        });
    }

    public void AddDataWithSorting(string levelID)
    {
        string urlData = $"{url}/ranking/{levelID}/playerDatas.json?auth={secret}";

        RestClient.Get(urlData).Then(response =>
        {
            Debug.Log(response.Text);
            JSONNode jsonNode = JSONNode.Parse(response.Text);

            ranking = new Ranking();
            ranking.playerDatas = new List<PlayerData>();
            for (int i = 0; i < jsonNode.Count; i++)
            {
                ranking.playerDatas.Add(new PlayerData(
                    rankNumber: jsonNode[i]["rankNumber"],
                    playerName: jsonNode[i]["playerName"],
                    playerTime: jsonNode[i]["playerTime"],
                    profileSprite: null));
            }

            PlayerData checkPlayerData = ranking.playerDatas.FirstOrDefault(data => data.playerName == currentPlayerData.playerName);
            int indexOfPlayer = ranking.playerDatas.IndexOf(checkPlayerData);

            if (checkPlayerData.playerName != null)
            {
                checkPlayerData.playerTime = currentPlayerData.playerTime;
                ranking.playerDatas[indexOfPlayer] = checkPlayerData;
            }
            else
            {
                ranking.playerDatas.Add(currentPlayerData);
            }

            CalculateRankFromScore();

            string urlPlayerData = $"{url}/ranking/{levelID}.json?auth={secret}";

            RestClient.Put<Ranking>(urlPlayerData, ranking).Then(response =>
            {
                Debug.Log("Upload Data Complete");
                rankUIManager.playerDatas = ranking.playerDatas;
                rankUIManager.ReloadRankData();
                FindYourDataInRanking();
            }).Catch(error =>
            {
                Debug.Log("error on set to server");
            });
        }).Catch(error =>
        {
            Debug.Log("Error to get data from server");
        });
    }
}
