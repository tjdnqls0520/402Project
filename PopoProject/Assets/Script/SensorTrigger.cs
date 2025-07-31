using UnityEngine;
using System.Collections;

public class SensorTrigger : MonoBehaviour
{
    public string trapTag = "SensorTrap"; // ������ Ʈ�� �±�
    public float moveDistance = 2f;     // �̵� �Ÿ�
    public float moveSpeed = 2f;        // �̵� �ӵ�
    public bool moveUp = true;          // ���� �̵����� �Ʒ��� �̵�����
    public bool loop = false;           // �ݺ� �̵� ����
    public bool autoLoop = false;       // ���� ���� �� �ڵ� �ݺ� ���� �߰�!

    private bool activated = false;

    void Start()
    {
        if (autoLoop && loop)
        {
            // �ڽ��� �ڽ� �� trapTag�� ���� �͸� �۵�
            foreach (Transform child in transform)
            {
                if (child.CompareTag(trapTag))
                {
                    Vector3 direction = Vector3.up * (moveUp ? moveDistance : -moveDistance);
                    StartCoroutine(LoopMoveTrap(child, direction));
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;

            // �ڽ��� �ڽ� �� trapTag�� ���� �͸� �۵�
            foreach (Transform child in transform)
            {
                if (child.CompareTag(trapTag))
                {
                    Vector3 direction = Vector3.up * (moveUp ? moveDistance : -moveDistance);
                    Vector3 targetPos = child.position + direction;

                    if (loop)
                    {
                        StartCoroutine(LoopMoveTrap(child, direction));
                    }
                    else
                    {
                        StartCoroutine(MoveTrap(child, targetPos));
                    }
                }
            }
        }
    }

    private IEnumerator MoveTrap(Transform obj, Vector3 target)
    {
        while (Vector3.Distance(obj.position, target) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator LoopMoveTrap(Transform obj, Vector3 direction)
    {
        Vector3 start = obj.position;
        Vector3 end = start + direction;

        while (true)
        {
            // �̵�
            while (Vector3.Distance(obj.position, end) > 0.01f)
            {
                obj.position = Vector3.MoveTowards(obj.position, end, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // ���� ����
            Vector3 temp = start;
            start = end;
            end = temp;
        }
    }
}