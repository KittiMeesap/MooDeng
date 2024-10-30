using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestScreenshot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TakeScreenshotAndShare());
        }
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        Debug.Log("Start Screenshot");
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared_img.jpg");
        File.WriteAllBytes(filePath, screenshot.EncodeToJPG());

        // To avoid memory leaks
        Destroy(screenshot);

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Hello world!")
            .SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log($"Share result: {result}, selected app: {shareTarget}"))
            .Share();
    }
}
