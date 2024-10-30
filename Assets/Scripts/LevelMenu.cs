using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] button;
    public GameObject levelButton;

    private void Awake()
    {
        ButtonToArray();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel",1);
        for (int i = 0; i < button.Length; i++)
        {
            button[i].interactable = false;
        }

        for (int i = 0; i < unlockedLevel; i++)
        {
            button[i].interactable = true;
        }
    }
    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        SceneManager.LoadScene(levelName);
    }

    void ButtonToArray()
    {
        int childCount = levelButton.transform.childCount;
        button = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            button[i] = levelButton.transform.GetChild(i).GetComponent<Button>();
        }
    }
}
