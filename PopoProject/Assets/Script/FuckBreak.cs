using UnityEngine;

public class FuckBreak : MonoBehaviour
{
    public GameObject pl;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pl == null) return;

        var player = pl.GetComponent<PlayerMouseMovement>();
        if (player != null && player.isBoostFlying && other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}
