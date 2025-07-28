// ================================================
// PlayerMouseMovement.cs (주석 버전)
// - 플레이어의 마우스 / 키보드 입력 기반 점프, 대시, 포물선 이동 구현
// - 크리스탈 아이템 기반 Boost(점프/대시) 시스템 포함
// ================================================

using System.Collections;
using UnityEngine;

public class PlayerMouseMovement : MonoBehaviour
{
    // === 외부 설정 변수 ===
    public Rigidbody2D rb;
    public AnimationCurve jumpArc; // 점프 포물선 궤적
    public float maxHoldTime = 0.2f; // 비행 유지 최대 시간
    public float maxDistance = 10f; // 비행 시 최대 거리
    public float arcHeight = 20f; // 비행 시 Y축 궤적 최대치
    public float dashSpeed = 12f; // 대시 속도
    public float jumpHeight = 8f; // 점프 높이
    public float jumpDuration = 0.2f; // 점프 지속 시간
    public LayerMask groundLayer; // 바닥 판정용 레이어

    // === 내부 상태 ===
    private bool isFlying = false;
    private Vector2 flyDirection;
    private float holdStartTime;
    private bool leftFlying, rightFlying;
    private bool leftHeld, rightHeld;
    private float leftClickTime, rightClickTime;
    private float doubleClickThreshold = 0.3f; // 더블클릭 허용 간격

    private bool isDashing = false;
    private bool isJumping = false;
    private bool isRepeatQueued = false;
    private bool spaceHeld = false;
    private bool doubleClickQueued = false;
    private int queuedDirection = 0; // -1 = 왼쪽, 1 = 오른쪽
    private enum InputDirection { None, Left, Right }
    private InputDirection lastClickDir = InputDirection.None;


    public enum BoostType { None, Dash, Jump }
    private BoostType currentBoost = BoostType.None;

    void Awake()
    {
        // Rigidbody2D 연결
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool grounded = IsGrounded();
        spaceHeld = Input.GetKey(KeyCode.Space);

        // 입력 처리 (마우스 or 키보드)
        bool leftInputDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A);
        bool rightInputDown = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.S);
        bool leftInputHeld = Input.GetMouseButton(0) || Input.GetKey(KeyCode.A);
        bool rightInputHeld = Input.GetMouseButton(1) || Input.GetKey(KeyCode.S);

        leftHeld = leftInputHeld;
        rightHeld = rightInputHeld;

        // 왼쪽 입력 더블클릭 시 Boost 발동
        if (leftInputDown)
        {
            if (lastClickDir == InputDirection.Left && Time.time - leftClickTime < doubleClickThreshold && currentBoost != BoostType.None)
            {
                if (currentBoost == BoostType.Dash && !isDashing)
                    StartCoroutine(PerformDash(-1));
                else if (currentBoost == BoostType.Jump && !isJumping)
                    StartCoroutine(PerformSmoothJump());

                currentBoost = BoostType.None;
                lastClickDir = InputDirection.None;
            }
            else
            {
                lastClickDir = InputDirection.Left;
                leftClickTime = Time.time;
            }
        }

        if (rightInputDown)
        {
            if (lastClickDir == InputDirection.Right && Time.time - rightClickTime < doubleClickThreshold && currentBoost != BoostType.None)
            {
                if (currentBoost == BoostType.Dash && !isDashing)
                    StartCoroutine(PerformDash(1));
                else if (currentBoost == BoostType.Jump && !isJumping)
                    StartCoroutine(PerformSmoothJump());

                currentBoost = BoostType.None;
                lastClickDir = InputDirection.None;
            }
            else
            {
                lastClickDir = InputDirection.Right;
                rightClickTime = Time.time;
            }
        }


        // 공중에서 양쪽 입력 시 낙하
        // 공중에서만 양쪽 입력으로 낙하 실행
        if (!grounded && leftInputDown && rightInputDown && !isDashing && !isJumping && !isFlying)
        {
            StartFall();
        }

        // 착지 상태에서 포물선 비행 시작
        if (!isFlying && !isDashing && !isJumping && grounded)
        {
            if (leftHeld && !rightHeld)
            {
                StartFlying(Vector2.left);
                leftFlying = true;
            }
            else if (rightHeld && !leftHeld)
            {
                StartFlying(Vector2.right);
                rightFlying = true;
            }
        }

        // 입력 해제 시 비행 종료
        if (leftFlying && !leftHeld) EndFlying();
        if (rightFlying && !rightHeld) EndFlying();
    }

    void FixedUpdate()
    {
        if (isFlying)
        {
            float holdTime = Mathf.Clamp(Time.time - holdStartTime, 0f, maxHoldTime);
            float t = holdTime / maxHoldTime;

            if (holdTime >= maxHoldTime)
            {
                EndFlying();
                return;
            }

            // 포물선 이동
            float x = flyDirection.x * Mathf.Lerp(2f, maxDistance, t);
            float y = jumpArc.Evaluate(t) * arcHeight;
            rb.linearVelocity = new Vector2(x, y);
        }
        else if (!isDashing && !isJumping && IsGrounded())
        {
            // 착지 상태 기본 이동 없음
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            // 다시 비행 조건 체크
            if (leftHeld && !rightHeld && !isFlying)
            {
                StartFlying(Vector2.left);
                leftFlying = true;
            }
            else if (rightHeld && !leftHeld && !isFlying)
            {
                StartFlying(Vector2.right);
                rightFlying = true;
            }
        }
    }

    // 포물선 이동 시작
    void StartFlying(Vector2 direction)
    {
        isFlying = true;
        holdStartTime = Time.time;
        flyDirection = direction.normalized;
        rb.gravityScale = 0f;
    }

    // 포물선 이동 종료
    void EndFlying()
    {
        isFlying = false;
        leftFlying = rightFlying = false;
        rb.gravityScale = 2f;

        // 반복 비행 예약
        if (IsGrounded() && !isJumping)
        {
            if (leftHeld && !rightHeld)
            {
                isRepeatQueued = true;
                StartCoroutine(ResumeFlying(Vector2.left));
            }
            else if (rightHeld && !leftHeld)
            {
                isRepeatQueued = true;
                StartCoroutine(ResumeFlying(Vector2.right));
            }
        }
    }

    // 다음 프레임에서 다시 비행 시작 (반복 입력 대응)
    IEnumerator ResumeFlying(Vector2 dir)
    {
        yield return null;
        if (isRepeatQueued && IsGrounded() && !isFlying && !isJumping)
        {
            StartFlying(dir);
            if (dir.x < 0) leftFlying = true;
            else rightFlying = true;
        }
        isRepeatQueued = false;
    }

    // 낙하 시작
    void StartFall()
    {
        isFlying = false;
        rb.gravityScale = 5f;
        rb.linearVelocity = Vector2.down * 20f;
    }

    // 대시 코루틴
    IEnumerator PerformDash(int direction)
    {
        isDashing = true;
        isDashing = false;
        rb.gravityScale = 0f;

        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + Vector2.right * direction * dashSpeed;
        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            Vector2 pos = Vector2.Lerp(startPos, endPos, t);
            rb.MovePosition(pos);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(endPos);
        rb.linearVelocity = Vector2.zero;

        // 체공 시간 (0.2초)
        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity = Vector2.down * 2f;
        rb.gravityScale = 2f;
        
    }

    // 점프 코루틴
    IEnumerator PerformSmoothJump()
    {
        isJumping = true;
        isJumping = false;
        rb.gravityScale = 0f;

        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + Vector2.up * jumpHeight;
        float timer = 0f;

        while (timer < jumpDuration)
        {
            float t = timer / jumpDuration;
            Vector2 pos = Vector2.Lerp(startPos, endPos, t);
            rb.MovePosition(pos);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(endPos);
        rb.linearVelocity = Vector2.zero;

        // 체공 시간 (0.2초)
        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity = Vector2.down * 2f;
        rb.gravityScale = 2f;

    }



    // 크리스탈 효과 등록
    public void SetBoost(BoostType type)
    {
        currentBoost = type;

        // 더블클릭 타이머 초기화
        leftClickTime = 0f;
        rightClickTime = 0f;
    }

    // 크리스탈 충돌 판정
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CrystalDash"))
        {
            SetBoost(BoostType.Dash);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("CrystalJump"))
        {
            SetBoost(BoostType.Jump);
            other.gameObject.SetActive(false);
        }
    }

    // 바닥 체크
    bool IsGrounded()
    {
        float rayDistance = 0.7f;
        Vector2 origin = transform.position + Vector3.down * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, groundLayer);
        Debug.DrawRay(origin, Vector2.down * rayDistance, hit.collider ? Color.green : Color.red);
        return hit.collider != null;
    }
}
