using UnityEngine;
using System.Collections;

public class TrapMover : MonoBehaviour
{
    public void Activate(Vector3 target, float speed)
    {
        StartCoroutine(MoveTrap(target, speed));
    }

    private IEnumerator MoveTrap(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
