using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CustomScreenshot : MonoBehaviour
{
    public string gameName = "MooDeng Adventure";

    public RawImage showImg;
    private byte[] currentTexture;
    private string currentFilePath;

    public GameObject screenshotPanel;
    public GameObject capturePanel;
    public GameObject saveImagePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string ScreenShotName()
    {
        return $"{gameName}_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpg";
    }

    public void Capture()
    {
        StartCoroutine(TakeScreenshot());
    }
    private IEnumerator TakeScreenshot()
    {
        EnableUICapture(false);
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        currentFilePath = Path.Combine(Application.temporaryCachePath, "temp_img.jpg");
        currentTexture = screenshot.EncodeToJPG();
        File.WriteAllBytes(currentFilePath, currentTexture);

        ShowImage();
        EnableUICapture(true);
        // To avoid memory leaks
        Destroy(screenshot);


    }

    public void EnableUICapture(bool isActive)
    {
        capturePanel.SetActive(isActive);
    }

    public void ShowImage()
    {
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.LoadImage(currentTexture);
        showImg.material.mainTexture = tex;
        screenshotPanel.SetActive(true);
    }

    public void ShareImage()
    {
        new NativeShare().AddFile(currentFilePath)
            .SetSubject("Subject goes here").SetText("Hello world!")
            .SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log($"Share result: {result}, selected app: {shareTarget}"))
            .Share();
    }
    public void SaveToGallery()
    {
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(currentFilePath, gameName, ScreenShotName(),
        (success, path) =>
        {
            Debug.Log("Media save result: " + success + " " + path);
            if (success)
            {
                saveImagePanel.SetActive(true);
            }

#if UNITY_EDITOR
            string editorFilePath = Path.Combine(Application.persistentDataPath, ScreenShotName());
            File.WriteAllBytes(editorFilePath, currentTexture);
#endif
        });

        Debug.Log("Permission result: " + permission);
    }

}
