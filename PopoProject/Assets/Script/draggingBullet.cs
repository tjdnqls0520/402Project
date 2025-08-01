using UnityEngine;

public class DraggingBullet : MonoBehaviour
{
    public float lifeTime = 10f;
    public LayerMask wallLayer;
    public float knockbackForce = 30f;  // �о �� ũ��
    public Vector2 forceDirection = Vector2.right; // �⺻ ���� (�Ҹ� ���� ������� �ٲ� ���� ����)

    private Rigidbody2D rb;
    private bool hasHitPlayer = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
        forceDirection = direction.normalized; // �Ҹ� ���� ���
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHitPlayer) return;

        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
            PlayerMouseMovement playerScript = collision.collider.GetComponent<PlayerMouseMovement>();

            if (playerRb != null)
            {
                // ���� �ӵ� �����ϰ� �� �� ���ϰ� ���� ��
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }

            hasHitPlayer = true;

            Destroy(gameObject, 0.1f); // ��� ���� (or �ణ�� ������)
        }

        // ���� �ε����� �׳� �ı�
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
