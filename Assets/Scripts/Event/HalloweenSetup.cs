using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalloweenSetup : MonoBehaviour
{
    [Header("Normal")]
    public GameObject normalPickUp;
    public GameObject halloweenPickUp;
    public GameObject normalPlayer;
    public GameObject halloweenPlayer;
    public GameObject normalMap1;
    public GameObject halloweenMap1;
    public GameObject normalMap2;
    public GameObject halloweenMap2;
    public GameObject normalBG;
    public GameObject halloweenBG;

    public void SetupHalloweenTheme(bool isActive)
    {
        halloweenPlayer.SetActive(isActive);
        normalPlayer.SetActive(!isActive);
        halloweenPickUp.SetActive(isActive);
        normalPickUp.SetActive(!isActive);
        halloweenMap1.SetActive(isActive);
        normalMap1.SetActive(!isActive);
        halloweenMap2.SetActive(isActive);
        normalMap2.SetActive(!isActive);
        halloweenBG.SetActive(isActive);
        normalBG.SetActive(!isActive);
    }

}
