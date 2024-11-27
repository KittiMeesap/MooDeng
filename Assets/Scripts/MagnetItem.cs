using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    public float magnetRadius = 5f; // ������������
    public float magnetSpeed = 10f; // ��������㹡�ôٴ�����
    public string itemTag = "Coin"; // Tag �ͧ���������ͧ��ôٴ

    private bool isMagnetActive = false; // ʶҹ��������
    private SpriteRenderer spriteRenderer; // ������Ѻ����¹�� Player
    private Color originalColor; // ��������ͧ Player

    void Start()
    {
        // ���� SpriteRenderer � Object �١
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // �ѹ�֡������ͧ Player
        }
        else
        {
            Debug.LogError("SpriteRenderer not found in child objects!");
        }
    }

    void Update()
    {
        if (isMagnetActive)
        {
            // ��������������շ���� Tag �ç�ѹ
            Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, magnetRadius);
            foreach (Collider2D item in items)
            {
                if (item.CompareTag(itemTag))
                {
                    // �֧���������Ҽ�����
                    Vector2 direction = (transform.position - item.transform.position).normalized;
                    item.transform.position += (Vector3)(direction * magnetSpeed * Time.deltaTime);

                    // ��������������Ǽ�����
                    if (Vector2.Distance(item.transform.position, transform.position) < 0.5f)
                    {
                        Debug.Log($"Item {item.name} collected!");
                        Destroy(item.gameObject); // ����������
                    }
                }
            }
        }
    }

    public void ActivateMagnet(float duration)
    {
        // �Դ��ҹ�������
        isMagnetActive = true;
        Debug.Log("Magnet activated!");

        // ����¹�� Player ������ǧ
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.magenta;
        }

        // �Դ���������ѧ�ҡ�������
        Invoke(nameof(DeactivateMagnet), duration);
    }

    private void DeactivateMagnet()
    {
        // �Դ�����ҹ�������
        isMagnetActive = false;

        // �׹�� Player �������
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        Debug.Log("Magnet deactivated.");
    }

    private void OnDrawGizmosSelected()
    {
        // �ʴ�������������� Scene View
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
