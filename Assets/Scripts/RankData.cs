using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public struct PlayerData
{
    public string playerName;
    public int rankNumber;
    public float playerTime;
    public Sprite profileSprite;

    public PlayerData(int rankNumber, string playerName, float playerTime, Sprite profileSprite)
    {
        this.playerName = playerName;
        this.rankNumber = rankNumber;
        this.playerTime = playerTime;
        this.profileSprite = profileSprite;
    }

}



public class RankData : MonoBehaviour
{
    public PlayerData playerData;

    [SerializeField] private Image imgPlayer; 
    [SerializeField] private TMP_Text rankPlayerText;
    [SerializeField] private TMP_Text namePlayerText; 
    [SerializeField] private TMP_Text timeText; 

    public void UpdateData()
    {
        imgPlayer.sprite = playerData.profileSprite;
        rankPlayerText.text = playerData.rankNumber.ToString();
        namePlayerText.text = playerData.playerName;
        timeText.text = playerData.playerTime.ToString("00:00");
    }
}
