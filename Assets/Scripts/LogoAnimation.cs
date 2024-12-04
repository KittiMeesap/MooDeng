using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    public float minScale = 0.5f; // ��Ҵ����ش
    public float maxScale = 1.5f; // ��Ҵ�����ش
    public float speed = 2.0f; // ��������㹡����͢��� (������Ҥ���������������Ѻ�����)

    void Update()
    {
        // �ӹǳ��Ҵ����������� Mathf.PingPong
        float scale = Mathf.PingPong(Time.time * speed, maxScale - minScale) + minScale;

        // ��Ѻ��Ҵ�ͧ GameObject �����ҷ��ӹǳ
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
