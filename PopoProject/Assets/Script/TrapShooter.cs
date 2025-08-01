using UnityEngine;

public class TrapShooter : MonoBehaviour
{
    public GameObject draggingBulletPrefab;
    public GameObject trapBulletPrefab;
    public GameObject homingBulletPrefab;

    public enum BulletType { Dragging, Trap, Homing }
    public BulletType currentBulletType = BulletType.Trap;

    public float interval = 1f;
    public float speed = 5f;
    public Vector2 direction = Vector2.left;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = null;

        switch (currentBulletType)
        {
            case BulletType.Dragging:
                bullet = Instantiate(draggingBulletPrefab, transform.position, Quaternion.identity);
                var drag = bullet.GetComponent<DraggingBullet>();
                if (drag != null)
                    drag.Launch(direction, speed);
                else
                    Debug.LogWarning("DraggingBullet ��ũ��Ʈ ����!");
                break;

            case BulletType.Trap:
                bullet = Instantiate(trapBulletPrefab, transform.position, Quaternion.identity);
                var trap = bullet.GetComponent<TrapProjectile>();
                if (trap != null)
                    trap.Launch(direction, speed);
                else
                    Debug.LogWarning("TrapProjectile ��ũ��Ʈ ����!");
                break;

            case BulletType.Homing:
                bullet = Instantiate(homingBulletPrefab, transform.position, Quaternion.identity);
                var homing = bullet.GetComponent<HomingBullet>();
                if (homing != null)
                    homing.Launch(direction, speed); // ���� �ѱ��!!
                else
                    Debug.LogWarning("HomingBullet ��ũ��Ʈ ����!");
                break;
        }
    }
}
