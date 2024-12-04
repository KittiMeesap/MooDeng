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
    [SerializeField] TMP_Text welcometext; // ใช้แสดงข้อความต้อนรับและสถานะ

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
        // สร้างเลขสุ่ม 5 หลัก
        int randomNum = Random.Range(10000, 99999);  // เลขสุ่มระหว่าง 10000 ถึง 99999

        // ตั้งค่าชื่อผู้เล่นเป็น "Guest" ตามด้วยเลขสุ่ม
        string guestName = "Guest" + randomNum.ToString();

        // บันทึกชื่อผู้เล่นที่เป็น Guest ลงใน PlayerPrefs
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

                // บันทึกชื่อผู้เล่นลงใน PlayerPrefs
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
        welcometext.text = ""; // เคลียร์ข้อความต้อนรับเมื่อกลับไปหน้าล็อกอิน
    }

    public void OpenRegister()
    {
        UILogin.gameObject.SetActive(false);
        UIRegister.gameObject.SetActive(true);
        welcometext.text = ""; // เคลียร์ข้อความต้อนรับเมื่อกลับไปหน้าลงทะเบียน
    }

    private IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(3f); // รอ 3 วินาที
        welcometext.text = "Loading menu..."; // แจ้งสถานะการเปลี่ยนซีน
        SceneManager.LoadScene("Menu"); // ใส่ชื่อซีนที่ต้องการจะเปลี่ยน
    }

    private void Awake()
    {
        SoundManager.instance.PlaySFX(SoundManager.instance.welcomeClip);
        SoundManager.instance.PlayBGM(SoundManager.instance.soundtrackClip);
    }
}
