using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMouseMovement : MonoBehaviour
{
    // === 필수 컴포넌트 / 레이어 ===
    public Rigidbody2D rb;
    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask eventLayer;   // 바닥 이벤트(브레이커블/트랩 등)
    public LayerMask trapLayer;    // 천장 트랩 체크

    // === 접지/레이 거리 ===
    [Header("Ray distances")]
    public float groundrayDistance = 1.3f;
    public float breakrayDistance = 1.4f;
    public float checkceilingtrap = 0.7f;

    // === 키보드 이동 파라미터 (아이워너 느낌) ===
    [Header("Keyboard Movement (IWB-style)")]
    [SerializeField] private float moveSpeed = 9.5f;
    [SerializeField] private float accel = 180f;
    [SerializeField] private float decel = 220f;
    [SerializeField] private float airAccel = 130f;
    [SerializeField] private float airDecel = 150f;
    [SerializeField] private float jumpVelocity = 11.8f;
    [SerializeField] private float gravityScaleNormal = 3.2f;
    [SerializeField] private float gravityScaleFall = 5.0f;
    [SerializeField] private float cutJumpFactor = 0.45f;
    [SerializeField] private float maxFallSpeed = -28f;
    [SerializeField] private float coyoteTime = 0.06f;
    [SerializeField] private float jumpBuffer = 0.08f;

    [Header("Ground Snap")]
    [SerializeField] private Collider2D bodyCollider; // 플레이어 메인 콜라이더
    [SerializeField] private float snapProbe = 0.20f;  // 발밑 레이 길이
    [SerializeField] private float snapSkin = 0.02f;  // 여유치

    // === 더블 점프 ===
    [Header("Extra Jumps")]
    [SerializeField] private int extraAirJumps = 1; // 2단 점프 = 공중 추가 1회
    private int airJumpsLeft = 0;

    // === 바운스 패널 관련(유지) ===
    [Header("Bounce Panels")]
    [SerializeField] private float bounceImpulseX = 12f;
    [SerializeField] private float bounceImpulseY = 15f;
    [SerializeField] private float inputLockAfterImpulse = 0.12f;
    [SerializeField] private float bounceProtectDuration = 0.06f;

    [Header("Slime Wall")]
    [SerializeField] private LayerMask slimeLayer;     // 슬라임(벽) 레이어
    [SerializeField] private float wallCheckDist = 0.18f; // (사용 안 함, 호환 유지용)
    [SerializeField] private float wallSlideMaxFall = -5.5f; // 슬라이드 최대 하강속도(음수)
    [SerializeField] private float wallJumpHorizontal = 9.0f; // 벽에서 튕겨나가는 X 속도
    [SerializeField] private float wallJumpVertical = 11.5f; // 벽점프 Y 속도
    [SerializeField] private bool requireSpaceForWallJump = false; // 스페이스도 같이 눌러야 벽점프? (기본 false)
    [SerializeField] private bool resetAirJumpsOnWallJump = true;  // 벽점프 후 공중점프 회복 여부

    // === 내부 상태 ===
    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;
    private float rawX = 0f;
    private bool jumpHeld = false;
    private float inputLockUntil = -999f;
    private bool isBouncing = false;
    private float bounceProtectUntil = -999f;
    bool touchingLeftSlime, touchingRightSlime;

    // === 스케일/방향 (기존 외부 의존 고려해 유지) ===
    public float dir = 1f;
    public bool dirseto = true;
    public bool chasize = true;
    public float dirsetofl = 1f;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!bodyCollider) bodyCollider = GetComponent<Collider2D>();

        // 튜널링 방지 핵심
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        bool locked = Time.time < inputLockUntil;

        // 좌/우 입력
        float left = (!locked && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))) ? -1f : 0f;
        float right = (!locked && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) ? 1f : 0f;
        rawX = Mathf.Clamp(left + right, -1f, 1f);

        // 점프 입력 버퍼
        if (!locked && Input.GetKeyDown(KeyCode.Space))
            lastJumpPressedTime = Time.time;
        jumpHeld = !locked && Input.GetKey(KeyCode.Space);

        // 접지 기록(코요테) + 공중점프 회복
        bool grounded = IsGrounded() || IsBreak();
        if (grounded)
        {
            lastGroundedTime = Time.time;
            airJumpsLeft = extraAirJumps; // 지상 접지 시 공중점프 회복
        }

        // 방향 뒤집기
        if (rawX != 0f)
        {
            dir = rawX > 0 ? 1f : -1f;
            transform.localScale = new Vector3(dirseto ? dir : dir * 0.5f, dirsetofl, dirsetofl);
        }

        // 천장 트랩 체크(즉사)
        CheckCeilingTrap();

        // 바닥 트랩 즉사
        var breakHit = IsBreak();
        if (breakHit.collider != null && breakHit.collider.CompareTag("Trap"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // (사이즈 프리셋 유지용)
        if (chasize)
        {
            dirseto = true;
            dir = Mathf.Sign(dir == 0 ? 1f : dir);
            dirsetofl = 1f;
            groundrayDistance = 1.3f;
            breakrayDistance = 1.4f;
            checkceilingtrap = 0.7f;
        }
        else
        {
            dirseto = false;
            dir = 0.5f;
            dirsetofl = 0.5f;
            groundrayDistance = 0.6f;
            breakrayDistance = 0.6f;
            checkceilingtrap = 0.35f;
        }

        // ==== Slime 벽 접촉 상태 갱신 (Cast 기반) ====
        // eventLayer 때문에 공중에서도 접지로 오인되는 걸 막기 위해, 벽 게이트는 IsGrounded()만 사용
        bool groundedForWall = IsGrounded();
        touchingLeftSlime = !groundedForWall && TouchingSlimeSideCast(-1);
        touchingRightSlime = !groundedForWall && TouchingSlimeSideCast(+1);

        // ==== 벽점프 입력(반대 방향키) ====
        bool awayLeft = touchingRightSlime && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow));
        bool awayRight = touchingLeftSlime && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));
        bool spaceOK = requireSpaceForWallJump ? Input.GetKey(KeyCode.Space) : true;

        if (!groundedForWall && spaceOK && (awayLeft || awayRight))
        {
            Vector2 v = rb.linearVelocity;
            if (awayLeft) v.x = -Mathf.Abs(wallJumpHorizontal);  // 왼쪽으로 튕김
            if (awayRight) v.x = Mathf.Abs(wallJumpHorizontal);  // 오른쪽으로 튕김
            v.y = wallJumpVertical;
            rb.linearVelocity = v;

            if (resetAirJumpsOnWallJump)
                airJumpsLeft = extraAirJumps;
        }
    }

    void FixedUpdate()
    {
        // 바운스 보호중이면 물리 최소화
        if (isBouncing && Time.time < bounceProtectUntil) return;
        isBouncing = false;

        Vector2 v = rb.linearVelocity;
        bool grounded = IsGrounded() || IsBreak();

        // 목표 속도 & 가감속(아이워너 반응성)
        float targetX = rawX * moveSpeed;
        float a = grounded
            ? (Mathf.Sign(targetX) == Mathf.Sign(v.x) ? accel : decel)
            : (Mathf.Abs(targetX) > Mathf.Abs(v.x) ? airAccel : airDecel);
        v.x = Mathf.MoveTowards(v.x, targetX, a * Time.fixedDeltaTime);

        // ===== 점프 처리 (코요테 + 버퍼 + 더블 점프) =====
        bool buffered = (Time.time - lastJumpPressedTime) <= jumpBuffer;
        bool canCoyote = (Time.time - lastGroundedTime) <= coyoteTime;

        if (buffered && (canCoyote || airJumpsLeft > 0))
        {
            v.y = jumpVelocity;

            if (!canCoyote && !grounded)
                airJumpsLeft = Mathf.Max(airJumpsLeft - 1, 0);

            lastJumpPressedTime = -999f;
            lastGroundedTime = -999f;
        }

        // 짧점
        if (!grounded && !jumpHeld && v.y > 0f)
            v.y *= cutJumpFactor;

        // 중력/낙하
        rb.gravityScale = (v.y < 0f) ? gravityScaleFall : gravityScaleNormal;
        if (v.y < maxFallSpeed) v.y = maxFallSpeed;

        rb.linearVelocity = v;

        // ==== Slime 벽 슬라이드 (천천히 떨어지게) ====
        bool groundedF = IsGrounded() || IsBreak();
        if (!groundedF && (touchingLeftSlime || touchingRightSlime))
        {
            Vector2 sv = rb.linearVelocity;
            if (sv.y > wallSlideMaxFall) // 예: -5.5f 보다 위면(느린 하강/정지)
                sv.y = Mathf.Max(sv.y - 0.5f, wallSlideMaxFall); // 부드럽게 끌어내림
            rb.linearVelocity = sv;
        }

        GroundSnap(ref v);
    }

    /* ===================== 충돌(크리스탈은 제거됨) ===================== */

    // 바운스/트랩 등
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 트랩 레이어 즉사
        if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        // 바운스 패널
        if (collision.collider.CompareTag("BounceLeftUp"))
        {
            Bounce(new Vector2(-bounceImpulseX, bounceImpulseY));
            return;
        }
        if (collision.collider.CompareTag("BounceRightUp"))
        {
            Bounce(new Vector2(+bounceImpulseX, bounceImpulseY));
            return;
        }
    }

    private void Bounce(Vector2 impulse)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(impulse, ForceMode2D.Impulse);

        isBouncing = true;
        bounceProtectUntil = Time.time + bounceProtectDuration;

        inputLockUntil = Time.time + inputLockAfterImpulse;

        // (옵션) 바운스 시 공중점프 회복
        // airJumpsLeft = Mathf.Max(airJumpsLeft, extraAirJumps);
    }

    private void GroundSnap(ref Vector2 vel)
    {
        if (vel.y > 0f) return;
        if (!bodyCollider) return;

        int groundMask = groundLayer | eventLayer;

        Bounds b = bodyCollider.bounds;
        Vector2 originCenter = new Vector2(b.center.x, b.min.y + 0.01f);
        float probe = snapProbe + snapSkin;

        Vector2 left = originCenter + Vector2.left * (b.extents.x * 0.6f);
        Vector2 right = originCenter + Vector2.right * (b.extents.x * 0.6f);

        RaycastHit2D hitC = Physics2D.Raycast(originCenter, Vector2.down, probe, groundMask);
        RaycastHit2D hitL = Physics2D.Raycast(left, Vector2.down, probe, groundMask);
        RaycastHit2D hitR = Physics2D.Raycast(right, Vector2.down, probe, groundMask);

        Debug.DrawRay(originCenter, Vector2.down * probe, hitC.collider ? Color.yellow : Color.gray);
        Debug.DrawRay(left, Vector2.down * probe, hitL.collider ? Color.yellow : Color.gray);
        Debug.DrawRay(right, Vector2.down * probe, hitR.collider ? Color.yellow : Color.gray);

        RaycastHit2D hit = hitC.collider ? hitC : (hitL.collider ? hitL : hitR);
        if (!hit.collider) return;

        float dist = hit.distance;
        if (dist > probe) return;

        float currentFootY = b.min.y;
        float targetFootY = hit.point.y + snapSkin;
        float delta = targetFootY - currentFootY;

        if (delta >= -0.001f)
        {
            rb.position += new Vector2(0f, delta);
            vel.y = Mathf.Max(vel.y, 0f);
            rb.linearVelocity = vel;
        }
    }

    // === 슬라임 접촉 감지: Collider.Cast 기반 (Edge/Composite OK, Trigger OK) ===
    bool TouchingSlimeSideCast(int sign) // -1=왼쪽, +1=오른쪽
    {
        if (!bodyCollider) return false;

        Vector2 dir = (sign < 0) ? Vector2.left : Vector2.right;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useLayerMask = true;
        filter.layerMask = slimeLayer;  // 인스펙터에서 반드시 지정!
        filter.useTriggers = true;      // 트리거도 감지

        RaycastHit2D[] hits = new RaycastHit2D[2];
        int count = bodyCollider.Cast(dir, filter, hits, 0.06f); // 아주 짧게 옆으로 캐스트
        if (count > 0) return true;

        // 보조(드물게 놓치는 경우 대비) - 얇은 OverlapBox
        Bounds b = bodyCollider.bounds;
        float padX = 0.04f;
        Vector2 size = new Vector2(0.12f, b.size.y * 0.8f);
        Vector2 center = (Vector2)b.center + new Vector2(sign * (b.extents.x + size.x * 0.5f + padX), 0f);
        bool boxHit = Physics2D.OverlapBox(center, size, 0f, slimeLayer);
#if UNITY_EDITOR
        Color c = (count > 0 || boxHit) ? Color.green : Color.red;
        Debug.DrawLine(center + Vector2.up * size.y * 0.5f, center - Vector2.up * size.y * 0.5f, c, 0f, false);
#endif
        return boxHit;
    }

    /* ===================== 레이 감지 유지 ===================== */

    public bool IsGrounded()
    {
        float rayDistance = groundrayDistance;

        Vector2 center = transform.position + Vector3.down * 0.2f;
        Vector2 left = center + Vector2.left * 0.1f;
        Vector2 right = center + Vector2.right * 0.1f;

        bool centerHit = Physics2D.Raycast(center, Vector2.down, rayDistance, groundLayer);
        bool leftHit = Physics2D.Raycast(left, Vector2.down, rayDistance, groundLayer);
        bool rightHit = Physics2D.Raycast(right, Vector2.down, rayDistance, groundLayer);

        Debug.DrawRay(center, Vector2.down * rayDistance, centerHit ? Color.green : Color.red);
        Debug.DrawRay(left, Vector2.down * rayDistance, leftHit ? Color.green : Color.red);
        Debug.DrawRay(right, Vector2.down * rayDistance, rightHit ? Color.green : Color.red);

        return centerHit || leftHit || rightHit;
    }

    public RaycastHit2D IsBreak()
    {
        float rayDistance = breakrayDistance;

        Vector2 center = transform.position + Vector3.down * 0.2f;
        Vector2 left = center + Vector2.left * 0.1f;
        Vector2 right = center + Vector2.right * 0.1f;

        RaycastHit2D centerHit = Physics2D.Raycast(center, Vector2.down, rayDistance, eventLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(left, Vector2.down, rayDistance, eventLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(right, Vector2.down, rayDistance, eventLayer);

        Debug.DrawRay(center, Vector2.down * rayDistance, centerHit.collider ? Color.cyan : Color.gray);
        Debug.DrawRay(left, Vector2.down * rayDistance, leftHit.collider ? Color.cyan : Color.gray);
        Debug.DrawRay(right, Vector2.down * rayDistance, rightHit.collider ? Color.cyan : Color.gray);

        if (centerHit.collider != null) return centerHit;
        if (leftHit.collider != null) return leftHit;
        if (rightHit.collider != null) return rightHit;

        return new RaycastHit2D();
    }

    private void CheckCeilingTrap()
    {
        float rayDistance = checkceilingtrap;
        Vector2 center = transform.position + Vector3.up * 0.5f;
        Vector2 left = center + Vector2.left * 0.1f;
        Vector2 right = center + Vector2.right * 0.1f;

        RaycastHit2D centerHit = Physics2D.Raycast(center, Vector2.up, rayDistance, trapLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(left, Vector2.up, rayDistance, trapLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(right, Vector2.up, rayDistance, trapLayer);

        Debug.DrawRay(center, Vector2.up * rayDistance, centerHit.collider ? Color.magenta : Color.gray);
        Debug.DrawRay(left, Vector2.up * rayDistance, leftHit.collider ? Color.magenta : Color.gray);
        Debug.DrawRay(right, Vector2.up * rayDistance, rightHit.collider ? Color.magenta : Color.gray);

        if ((centerHit.collider && centerHit.collider.CompareTag("Trap")) ||
            (leftHit.collider && leftHit.collider.CompareTag("Trap")) ||
            (rightHit.collider && rightHit.collider.CompareTag("Trap")))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
