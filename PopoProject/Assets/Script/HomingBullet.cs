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
            Debug.LogWarning("유도탄: 타겟이 없습니다!");
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
            Debug.Log("유도탄이 환경에 충돌하여 파괴됨");
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("플레이어 유도탄 맞고 즉사!!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
