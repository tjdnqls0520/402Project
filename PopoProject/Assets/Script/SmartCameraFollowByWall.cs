using UnityEngine;

public class SmartCameraFollowByWall : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float rayDistance = 8f;
    public float raygroundDistance = 4f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;

    private bool blockLeft, blockRight, blockUp;
    private Vector3 currentVelocity;

    void Update()
    {
        Vector3 targetPos = target.position;
        Vector3 cameraPos = transform.position;

        blockLeft = Physics2D.Raycast(cameraPos, Vector2.left, rayDistance, wallLayer);
        blockRight = Physics2D.Raycast(cameraPos, Vector2.right, rayDistance, wallLayer);
        blockUp = Physics2D.Raycast(cameraPos, Vector2.up, raygroundDistance, groundLayer);

        float targetX = cameraPos.x;
        float targetY = cameraPos.y;

        if (!blockLeft && target.position.x < cameraPos.x)
            targetX = target.position.x;
        else if (!blockRight && target.position.x > cameraPos.x)
            targetX = target.position.x;

        if (!blockUp && target.position.y > cameraPos.y)
            targetY = target.position.y;
        else if (target.position.y < cameraPos.y)
            targetY = target.position.y;

        Vector3 desiredPosition = new Vector3(targetX, targetY, cameraPos.z);
        transform.position = Vector3.SmoothDamp(cameraPos, desiredPosition, ref currentVelocity, 1f / followSpeed);
    }

    void OnDrawGizmos()
    {
        Vector3 cameraPos = transform.position;

        // 좌측 레이
        RaycastHit2D hitLeft = Physics2D.Raycast(cameraPos, Vector2.left, rayDistance, wallLayer);
        Gizmos.color = hitLeft.collider ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.left * rayDistance);

        // 우측 레이
        RaycastHit2D hitRight = Physics2D.Raycast(cameraPos, Vector2.right, rayDistance, wallLayer);
        Gizmos.color = hitRight.collider ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.right * rayDistance);

        // 위쪽 레이
        RaycastHit2D hitUp = Physics2D.Raycast(cameraPos, Vector2.up, raygroundDistance, groundLayer);
        Gizmos.color = hitUp.collider ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.up * raygroundDistance);
    }
}
