using UnityEngine;

public class SmartCameraFollowByWall : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float rayDistance = 8f;
    public float raygroundDistance = 4f;
    public float yOffset = 3f;              //추가: Y축 오프셋
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

        // 좌우 이동
        if (!blockLeft && target.position.x < cameraPos.x)
            targetX = target.position.x;
        else if (!blockRight && target.position.x > cameraPos.x)
            targetX = target.position.x;

        // 위쪽 제한 + 오프셋 적용
        float desiredY = target.position.y + yOffset;

        if (!blockUp && desiredY > cameraPos.y)
            targetY = desiredY;
        else if (desiredY < cameraPos.y)
            targetY = desiredY;

        Vector3 desiredPosition = new Vector3(targetX, targetY, cameraPos.z);
        transform.position = Vector3.SmoothDamp(cameraPos, desiredPosition, ref currentVelocity, 1f / followSpeed);
    }

    void OnDrawGizmos()
    {
        Vector3 cameraPos = transform.position;

        RaycastHit2D hitLeft = Physics2D.Raycast(cameraPos, Vector2.left, rayDistance, wallLayer);
        Gizmos.color = hitLeft.collider ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.left * rayDistance);

        RaycastHit2D hitRight = Physics2D.Raycast(cameraPos, Vector2.right, rayDistance, wallLayer);
        Gizmos.color = hitRight.collider ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.right * rayDistance);

        RaycastHit2D hitUp = Physics2D.Raycast(cameraPos, Vector2.up, raygroundDistance, groundLayer);
        Gizmos.color = hitUp.collider ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.up * raygroundDistance);
    }
}
