// ================================================
// DirScaleController.cs
// - 숫자 5번 키 입력 시 dir 값을 0.5f로 고정하고
//   PlayerMouseMovement의 dir, localScale 값을 조정하는 스크립트
// ================================================

using UnityEngine;

public class DirScaleController : MonoBehaviour
{
    public PlayerMouseMovement playerMovement;

    void Update()
    {
        // 숫자 5번 키 (KeyCode.Alpha5) 입력 감지
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (playerMovement != null)
            {
                // dir 값을 0.5f 또는 -0.5f로 현재 방향 유지하며 줄임
                float currentDir = playerMovement.GetDir();
                float newDir = Mathf.Sign(currentDir) * 0.5f;
                playerMovement.SetDir(newDir);

                // localScale 적용 (dir 방향 반영)
                playerMovement.transform.localScale = new Vector3(newDir, 1f, 1f);
                Debug.Log("Dir and localScale updated to half scale: " + newDir);
            }
        }
    }
}
