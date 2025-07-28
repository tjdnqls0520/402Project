using UnityEngine;

public class CrystalTrigger : MonoBehaviour
{
    public string boostType = "Dash"; // 또는 "Jump"

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMouseMovement player = collision.GetComponent<PlayerMouseMovement>();
            if (player != null)
            {
                if (boostType == "Dash")
                    player.SetBoost(PlayerMouseMovement.BoostType.Dash);
                else if (boostType == "Jump")
                    player.SetBoost(PlayerMouseMovement.BoostType.Jump);
            }

            gameObject.SetActive(false); // 크리스탈은 무조건 제거됨
        }
    }
}

