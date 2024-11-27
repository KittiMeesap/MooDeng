using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    public float magnetDuration = 3f; // �������ҷ��������硷ӧҹ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // �Դ��ҹ�������㹵�Ǽ�����
            MagnetItem magnetItem = collision.GetComponent<MagnetItem>();
            if (magnetItem != null)
            {
                magnetItem.ActivateMagnet(magnetDuration);
            }

            // �����������������
            Destroy(gameObject);
        }
    }
}
