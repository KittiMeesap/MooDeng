using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using System;
using UnityEngine.Purchasing.Extension;

public class IAPStore : MonoBehaviour
{
    [Header("Consumable")]
    public TextMeshProUGUI coinText;

    [Header("Non Consumable")]
    public GameObject adsPurchasedWindow;
    public GameObject adsBanner;

    [Header("Subscription")]
    public GameObject subActivatedWindow;
    public GameObject premiumLogo;
    public GameObject premiumBG;
    public GameObject premiumShop;
    public GameObject premiumUI1;
    public GameObject premiumUI2;
    public GameObject premiumUI3;

    // Start is called before the first frame update
    void Start()
    {
        coinText.text = PlayerPrefs.GetInt("totalCoins").ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning("Fail Test " + product.definition.id);
        Debug.LogWarning("Fail Test " + failureDescription.reason);
    }
    //Consumable
    public void OnPurchaseCoins100Complete(Product product)
    {
        Debug.Log(product.definition.id);
        AddCoin(100);

    }

    void AddCoin(int num)
    {
        int coins = PlayerPrefs.GetInt("totalCoins");
        coins += num;
        PlayerPrefs.SetInt("totalCoins", coins);
        coinText.text = coins.ToString();
    }

    //Non-Consumable
    void DisplayAds(bool active)
    {
        if (!active)
        {
            adsPurchasedWindow.SetActive(true);
            adsBanner.SetActive(false);
        }
        else
        {
            adsPurchasedWindow.SetActive(false);
            adsBanner.SetActive(true);
        }
    }

    void RemoveAds()
    {
        DisplayAds(false);
    }
    void ShowAds()
    {
        DisplayAds(true);
    }

    public void OnPurchaseRemoveAdsComplete(Product product)
    {
        Debug.Log(product.definition.id);
        RemoveAds();
    }

    public void CheckNonConsumale(Product product)
    {
        if(product != null)
        {
            if(product.hasReceipt)
            {
                RemoveAds();
            }
            else
            {
                ShowAds();
            }
        }
    }

    //Subscription
    void SetupBattlePass(bool active)
    {
        if (active)
        {
            subActivatedWindow.SetActive(true);
            premiumLogo.SetActive(true);
            premiumBG.SetActive(true);
            premiumShop.SetActive(true);
            premiumUI1.SetActive(true);
            premiumUI2.SetActive(true); 
            premiumUI3.SetActive(true); 
        }
        else
        {
            subActivatedWindow.SetActive(false);
            premiumLogo.SetActive(false);
            premiumBG.SetActive(false);
            premiumShop.SetActive(false);
            premiumUI1.SetActive(false);
            premiumUI2.SetActive(false);
            premiumUI3.SetActive(false);
        }
    }
    void ActivateBattlePass()
    {
        SetupBattlePass(true);
    }

    void DeactivateBattlePass()
    {
        SetupBattlePass(false);
    }

    public void OnPurchaseActivateBattlePassComplete(Product product)
    {
        Debug.Log(product.definition.id);
        ActivateBattlePass();
    }

    public void CheckSubscription(Product subProduct)
    {
        try
        {
            if (subProduct.hasReceipt)
            {
                var subManager = new SubscriptionManager(subProduct, null);
                var info = subManager.getSubscriptionInfo();

                if(info.isSubscribed() == Result.True)
                {
                    Debug.Log("We are subscribed");
                    ActivateBattlePass();
                }
                else
                {
                    Debug.Log("Unsubscribed");
                    DeactivateBattlePass();
                }
            }
            else 
            {
                Debug.Log("receipt not found !");
            }
        }
        catch (Exception) 
        {
            Debug.Log("It only work for Google store, app store,amazon store,you are using fake store!");
        }
    }
}
