using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using Models;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    [Header("Register")]
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    [SerializeField] TMP_InputField username;
    [SerializeField] GameObject UIRegister;
    [Header("Login")]
    [SerializeField] TMP_InputField loginemail;
    [SerializeField] TMP_InputField loginpassword;
    [SerializeField] GameObject UILogin;
    [SerializeField] TMP_Text welcometext;
    // Start is called before the first frame update
    public void OnRegister()
    {
        if (!string.IsNullOrEmpty(email.text) && !string.IsNullOrEmpty(password.text) && !string.IsNullOrEmpty(username.text))
        {
            AuthHandler.SignUp(email.text, password.text, new User(username.text));
        }
        else
        {
            Debug.Log("Please input your information!!!");
        }
    }

    public void OnLogin()
    {
        if (!string.IsNullOrEmpty(loginemail.text) && !string.IsNullOrEmpty(loginpassword.text))
        {
            AuthHandler.SignIn(loginemail.text, loginpassword.text, user =>
            {
                welcometext.text = "Welcome " + user.name;
                welcometext.gameObject.SetActive(true);

                // บันทึกชื่อผู้เล่นลงใน PlayerPrefs
                PlayerPrefs.SetString("PlayerName", user.name);
                PlayerPrefs.Save();

                StartCoroutine(WaitAndChangeScene());
            },
            () =>
            {
                Debug.Log("Email not verified or login failed");
            });
        }
        else
        {
            Debug.Log("Please input your email and password!!!");
        }
    }


    public void CloseRegister()
    {
        UILogin.gameObject.SetActive(true);
        UIRegister.gameObject.SetActive(false);
    }
    public void OpenRegister()
    {
        UILogin.gameObject.SetActive(false);
        UIRegister.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(3f); // รอ 3 วินาที
        SceneManager.LoadScene("Menu"); // ใส่ชื่อซีนที่ต้องการจะเปลี่ยน
    }
}
