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
    public const string url = "https://moodengleaderboard-default-rtdb.asia-southeast1.firebasedatabase.app";
    public const string secret = "2S8Q9HGudjIZuK1nmJJo56249y2XB51sFCxRESgx";

    [Header("Main")]
    public RankUIManager rankUIManager;
    public Ranking ranking;

    [Header("New Data")]
    public PlayerData currentPlayerData;
    private List<PlayerData> sortPlayerDatas = new List<PlayerData>();



    #region Test

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
        
        //TestSetData();
        //TestSetData2();
        //TestGetData();
        //TestGetData2();
        ReloadSortingData();
    }

    [ContextMenu("Set Local Data To Database")]
    public void SetLocalDataToDatabase()
    {
        string urlData = $"{url}/ranking.json?auth={secret}";
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
            changedRankNum.rankNumber = i + 1; // ÍÑ»à´µËÁÒÂàÅ¢ÅÓ´Ñº

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
    }

    public void ReloadSortingData()
    {
        string urlData = $"{url}/ranking/playerDatas.json?auth={secret}";

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

            string urlPlayerData = $"{url}/ranking/.json?auth={secret}";

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
        }).Catch(Error =>
        {
            Debug.Log("Error to get data from server");
        });
    }

    public void AddDataWithSorting()
    {
        string urlData = $"{url}/ranking/playerDatas.json?auth={secret}";

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

            string urlPlayerData = $"{url}/ranking/.json?auth={secret}";

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
