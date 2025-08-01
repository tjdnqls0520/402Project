using UnityEngine;

public class TrapShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireInterval = 1.5f;
    public float projectileSpeed = 5f;

    public bool up, down, left, right;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            timer = 0f;
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        Vector2 direction = Vector2.zero;

        if (up) direction += Vector2.up;
        if (down) direction += Vector2.down;
        if (left) direction += Vector2.left;
        if (right) direction += Vector2.right;

        if (direction == Vector2.zero) return;

        direction.Normalize();

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        TrapProjectile trap = proj.GetComponent<TrapProjectile>();
        trap.Launch(direction, projectileSpeed); // ← 이거 안 하면 절대 안 날아감
    }
}
