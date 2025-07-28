using UnityEngine;

public class MoveTest : MonoBehaviour
{
    private Rigidbody2D rb;
    void Start() => rb = GetComponent<Rigidbody2D>();

    void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(input * 10f, rb.linearVelocity.y);
        Debug.Log("Transform X: " + transform.position.x);
    }
}