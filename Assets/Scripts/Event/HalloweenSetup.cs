using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalloweenSetup : MonoBehaviour
{
    [Header("Normal")]
    public GameObject halloweenCoin;

    [System.Serializable]
    public struct ThemeSetupData
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupHalloweenTheme(bool isActive)
    {
        halloweenCoin.SetActive(isActive);
    }

}
