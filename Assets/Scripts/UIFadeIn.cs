using System.Collections;
using UnityEngine;

public class UIFadeIn : MonoBehaviour
{
    public float fadeDuration = 1f;  // �������ҷ����㹡��࿴
    private CanvasGroup canvasGroup; // CanvasGroup ���Ǻ������࿴

    private void OnEnable()
    {
        // �֧ CanvasGroup �Ҩҡ GameObject
        canvasGroup = GetComponent<CanvasGroup>();

        // ��Ǩ�ͺ��� CanvasGroup �������������
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0; // ��駤�����������������
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogWarning("����� CanvasGroup � GameObject ���");
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // �ӹǳ��� alpha ������ҷ���ҹ�
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);

            // �������ҷ���ҹ�
            elapsedTime += Time.deltaTime;

            yield return null; // ��仨���������Ѵ�
        }

        // ��駤�� alpha �� 1 ����͡��࿴��������ó�
        canvasGroup.alpha = 1;
    }
}
