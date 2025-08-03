using UnityEngine;

public class FuckBreak : MonoBehaviour
{
    public GameObject pl;
    public GameObject hitEffect; // 이펙트 프리팹
    public LayerMask groundLayer; // 바닥 Layer 설정 필요!

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pl == null) return;
        var player = pl.GetComponent<PlayerMouseMovement>();
        if (player == null) return;

        // 1. 부스트 상태에서 Bullet 감지 → 파괴
        if (player.dash && other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }

        // 2. 바닥과 충돌했는지 판단 (레이 안 씀)
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            if (player.dash)
            {
                GameObject fx = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(fx, 1f);
            }
        }
    }
}
