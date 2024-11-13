using System.Collections;
using UnityEngine;

public class CustomizableFadeScaleRotate : MonoBehaviour
{
    public bool enableFade = true;            // �Դ���ͻԴ���࿴
    public bool enableScale = true;           // �Դ���ͻԴ��â��¢�Ҵ
    public bool enableRotate = false;         // �Դ���ͻԴ�����ع

    public float duration = 1f;               // �������ҡ��࿴��Т��¢�Ҵ
    public Vector3 targetScale = Vector3.one; // ��Ҵ�ش���·���ͧ�������ʴ�
    public float rotationSpeed = 1080f;       // ��������㹡����ع (ͧ�ҵ���Թҷ�)

    private SpriteRenderer spriteRenderer;    // ������Ѻ�Ǻ�����Ҥ�������ʧ (alpha)

    private void OnEnable()
    {
        // ��駤��������鹢�Ҵ��� alpha �ͧ SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enableScale)
        {
            transform.localScale = Vector3.zero; // ��駤�� scale ��������� 0 ��ҵ�ͧ����Ϳ࿡����¢�Ҵ
        }

        if (spriteRenderer != null && enableFade)
        {
            Color color = spriteRenderer.color;
            color.a = 0; // ��駤�� alpha ��������� 0 ��ҵ�ͧ����Ϳ࿡��࿴
            spriteRenderer.color = color;
        }

        // ����� Coroutine ���������࿴ ���¢�Ҵ �����ع (��ҡ�˹�����Դ) �Դ���
        StartCoroutine(FadeScaleAndRotate(true));
    }

    // �ѧ��ѹ��������ͷӡ�� fade out �����͢�Ҵ �ҡ��鹤��»Դ�ѵ��
    public void FadeOutAndDeactivate()
    {
        StartCoroutine(FadeScaleAndRotate(false, true));
    }

    private IEnumerator FadeScaleAndRotate(bool isShowing, bool deactivateOnEnd = false)
    {
        float elapsedTime = 0f;
        Vector3 startScale = isShowing ? Vector3.zero : targetScale;
        Vector3 endScale = isShowing ? targetScale : Vector3.zero;
        float startAlpha = isShowing ? 0f : 1f;
        float endAlpha = isShowing ? 1f : 0f;

        while (elapsedTime < duration)
        {
            // �ӹǳ�����繵�ͧ�������¹�ŧ��������
            float progress = elapsedTime / duration;

            // ��Ѻ��Ҵ (�����������) �����ȷҧ����˹�
            if (enableScale)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            }

            // ��Ѻ��� alpha (����ʧ) ����Դ��ҹ���࿴
            if (spriteRenderer != null && enableFade)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(startAlpha, endAlpha, progress);
                spriteRenderer.color = color;
            }

            // ��ع�ѵ�ض�� enableRotate �� true
            if (enableRotate)
            {
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime * (isShowing ? 1 : -1));
            }

            // �������ҷ���ҹ�
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ��駤�Ң�Ҵ��� alpha ����繤�������������͡��࿴��Т����������
        if (enableScale)
        {
            transform.localScale = endScale;
        }

        if (spriteRenderer != null && enableFade)
        {
            Color color = spriteRenderer.color;
            color.a = endAlpha;
            spriteRenderer.color = color;
        }

        // ��ҡ����ع������� ����駤�� rotation ��Ѻ价�� (0, 0, 0)
        if (enableRotate)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // �Դ����ʴ��Ţͧ GameObject ��ҡ�˹����Դ����ͨ���÷ӧҹ
        if (deactivateOnEnd)
        {
            gameObject.SetActive(false);
        }
    }
}
