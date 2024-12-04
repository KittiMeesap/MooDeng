using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Models;

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

    [Header("Feedback")]
    [SerializeField] TMP_Text welcometext; // ���ʴ���ͤ�����͹�Ѻ���ʶҹ�

    public void OnRegister()
    {
        if (!string.IsNullOrEmpty(email.text) && !string.IsNullOrEmpty(password.text) && !string.IsNullOrEmpty(username.text))
        {
            AuthHandler.SignUp(email.text, password.text, new User(username.text));
            welcometext.text = "Registration successful! Please verify your email.";
        }
        else
        {
            welcometext.text = "Please input all registration information!";
        }
    }

    public void OnGuest()
    {
        // ���ҧ�Ţ���� 5 ��ѡ
        int randomNum = Random.Range(10000, 99999);  // �Ţ���������ҧ 10000 �֧ 99999

        // ��駤�Ҫ��ͼ������� "Guest" ��������Ţ����
        string guestName = "Guest" + randomNum.ToString();

        // �ѹ�֡���ͼ����蹷���� Guest ŧ� PlayerPrefs
        PlayerPrefs.SetString("PlayerName", guestName);
        PlayerPrefs.Save();

        welcometext.text = "Welcome " + guestName + "! Logging you in...";

        StartCoroutine(WaitAndChangeScene());
    }


    public void OnLogin()
    {
        if (!string.IsNullOrEmpty(loginemail.text) && !string.IsNullOrEmpty(loginpassword.text))
        {
            AuthHandler.SignIn(loginemail.text, loginpassword.text, user =>
            {
                welcometext.text = "Welcome " + user.name + "! Logging you in...";

                // �ѹ�֡���ͼ�����ŧ� PlayerPrefs
                PlayerPrefs.SetString("PlayerName", user.name);
                PlayerPrefs.Save();

                StartCoroutine(WaitAndChangeScene());
            },
            () =>
            {
                welcometext.text = "Login failed: Email not verified or incorrect credentials.";
            });
        }
        else
        {
            welcometext.text = "Please input your email and password!";
        }
    }

    public void CloseRegister()
    {
        UILogin.gameObject.SetActive(true);
        UIRegister.gameObject.SetActive(false);
        welcometext.text = ""; // �������ͤ�����͹�Ѻ����͡�Ѻ�˹����͡�Թ
    }

    public void OpenRegister()
    {
        UILogin.gameObject.SetActive(false);
        UIRegister.gameObject.SetActive(true);
        welcometext.text = ""; // �������ͤ�����͹�Ѻ����͡�Ѻ�˹��ŧ����¹
    }

    private IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(3f); // �� 3 �Թҷ�
        welcometext.text = "Loading menu..."; // ��ʶҹС������¹�չ
        SceneManager.LoadScene("Menu"); // �����ͫչ����ͧ��è�����¹
    }

    private void Awake()
    {
        SoundManager.instance.PlaySFX(SoundManager.instance.welcomeClip);
        SoundManager.instance.PlayBGM(SoundManager.instance.soundtrackClip);
    }
}
