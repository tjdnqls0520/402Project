using UnityEngine;
using UnityEngine.SceneManagement;

public class HomingBullet : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public float lifeTime = 10f;

    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public LayerMask eventLayer;

    private Rigidbody2D rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (target == null)
        {
            Debug.LogWarning("����ź: Ÿ���� �����ϴ�!");
            Destroy(gameObject);
        }

        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;

        bool hitWall = ((1 << layer) & wallLayer) != 0;
        bool hitGround = ((1 << layer) & groundLayer) != 0;
        bool hitEvent = ((1 << layer) & eventLayer) != 0;

        if (hitWall || hitGround || hitEvent)
        {
            Debug.Log("����ź�� ȯ�濡 �浹�Ͽ� �ı���");
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("�÷��̾� ����ź �°� ���!!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
