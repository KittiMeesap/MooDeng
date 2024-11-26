using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticManager : MonoBehaviour
{
    public static AnalyticManager Instance { get; private set; }
    private bool _isInitialized = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        _isInitialized = true;
    }

    public void StartLevel(string currentLevel)
    {
        if (!_isInitialized)
        {
            return;
        }

        CustomEvent myEvent = new CustomEvent("start_level")
    {
        {"level_index", currentLevel}
    };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log("start_level " + currentLevel);
    }

    public void FinishLevel(string currentLevel)
    {
        if (!_isInitialized)
        {
            return;
        }

        CustomEvent myEvent = new CustomEvent("finish_level")
    {
        {"level_index", currentLevel}
    };

        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log("finish_level " + currentLevel);
    }

    public void DeathRestartGame()
    {
        AnalyticsService.Instance.RecordEvent("death_restart_game");
        AnalyticsService.Instance.Flush();
        Debug.Log("death_restart_game");
    }



}
