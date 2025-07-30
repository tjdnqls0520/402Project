// ================================================
// DirScaleController.cs
// - ���� 5�� Ű �Է� �� dir ���� 0.5f�� �����ϰ�
//   PlayerMouseMovement�� dir, localScale ���� �����ϴ� ��ũ��Ʈ
// ================================================

using UnityEngine;

public class DirScaleController : MonoBehaviour
{
    public PlayerMouseMovement playerMovement;

    void Update()
    {
        // ���� 5�� Ű (KeyCode.Alpha5) �Է� ����
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (playerMovement != null)
            {
                // dir ���� 0.5f �Ǵ� -0.5f�� ���� ���� �����ϸ� ����
                float currentDir = playerMovement.GetDir();
                float newDir = Mathf.Sign(currentDir) * 0.5f;
                playerMovement.SetDir(newDir);

                // localScale ���� (dir ���� �ݿ�)
                playerMovement.transform.localScale = new Vector3(newDir, 1f, 1f);
                Debug.Log("Dir and localScale updated to half scale: " + newDir);
            }
        }
    }
}
