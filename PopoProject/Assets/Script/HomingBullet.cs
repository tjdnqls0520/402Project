using UnityEngine;
using UnityEngine.SceneManagement;

public class HomingBullet : MonoBehaviour
{
    public float speed = 5f;
    public float rotateSpeed = 200f;
    public float lifeTime = 10f;
    public LayerMask wallLayer;

    private Transform target;
    private Rigidbody2D rb;
    private Vector2 initialDirection = Vector2.right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.right).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.right * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("�÷��̾� ȣ�� źȯ �ǰ�!");

            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void Launch(Vector2 direction, float spd)
    {
        initialDirection = direction.normalized;
        speed = spd;
        transform.right = initialDirection; // �߻� �������� ȸ��
    }
}
