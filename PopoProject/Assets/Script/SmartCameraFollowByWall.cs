using UnityEngine;

public class SmartCameraFollowByWall : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10f;
    public float rayDistance = 8f;
    public float raygroundDistance = 4f;
    public float yOffset = 3f;
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

        // 원웨이 타일은 감지에서 제외
        RaycastHit2D hitUpRaw = Physics2D.Raycast(cameraPos, Vector2.up, raygroundDistance, groundLayer);
        blockUp = hitUpRaw.collider != null && hitUpRaw.collider.tag != "OneWay";

        float targetX = cameraPos.x;
        float targetY = cameraPos.y;

        if (!blockLeft && target.position.x < cameraPos.x)
            targetX = target.position.x;
        else if (!blockRight && target.position.x > cameraPos.x)
            targetX = target.position.x;

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
        Gizmos.color = hitUp.collider && hitUp.collider.tag != "OneWay" ? Color.blue : Color.red;
        Gizmos.DrawLine(cameraPos, cameraPos + Vector3.up * raygroundDistance);
    }
}
