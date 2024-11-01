using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalloweenSetup : MonoBehaviour
{
    [Header("Normal")]
    public GameObject normalPickUp;
    public GameObject halloweenPickUp;

    public void SetupHalloweenTheme(bool isActive)
    {
        halloweenPickUp.SetActive(isActive);
        normalPickUp.SetActive(!isActive);
    }

}
