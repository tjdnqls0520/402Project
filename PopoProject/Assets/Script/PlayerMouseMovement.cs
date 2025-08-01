using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMouseMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public AnimationCurve jumpArc;
    public float maxHoldTime = 0.2f;
    public float maxDistance = 10f;
    public float arcHeight = 20f;
    public float dashSpeed = 12f;
    public float jumpHeight = 8f;
    public float jumpDuration = 0.2f;
    public float wallJumpForceX = 1f;
    public float wallJumpForceY = 1f;
    public float rayLength = 0.8f;
    public LayerMask groundLayer;
    public LayerMask eventLayer;
    public LayerMask onewayLayer;
    public LayerMask trapLayer;
    public float rayRadius = 0.1f; // ← 레이의 '두께'를 담당해요!
    public bool isFlying = false;
    public bool getskill = false;
    public bool isBoostFlying = false; // ★ Boost 비행 중인지
    public bool dash = false;
    public bool jump = false;
    public bool dirseto = true;
    public float dirsetofl = 1f;
    public float groundrayDistance = 1.3f;
    public float breakrayDistance = 1.4f;
    public float WallrayDistance = 0.5f;
    public float raytrens = 0.7f;
    private Vector2 boostDirection = Vector2.zero; // ★ Boost 비행 방향

    private Vector2 flyDirection;
    private Animator ani;
    private float holdStartTime;
    public bool leftFlying, rightFlying;
    private bool leftHeld, rightHeld;
    private float leftClickTime, rightClickTime;
    private float doubleClickThreshold = 0.3f;
    private float boostStartTime;

    private float dir = 1f;
    private float timer = 0;

    private bool isDashing = false;
    private bool isJumping = false;
    private bool isRepeatQueued = false;
    private bool spaceHeld = false;
    private bool doubleClickQueued = false;
    private bool isFallevent = false;
    private int queuedDirection = 0;

    private enum InputDirection { None, Left, Right }
    private InputDirection lastClickDir = InputDirection.None;
    private float bothClickTime = 0f;
    private float bothClickThreshold = 0.1f;
    private float fallCooldown = 0.2f;
    private float fallLockUntil = 0f;
    private RaycastHit2D Hit;
    private float wallStickY = 0f; // 고정할 Y값
    private bool isWallJumping = false;
    private float wallSltickY = float.NaN;
    private bool wasJumping = false; // 점프 중이었는지 기억하는 변수
    private bool isLanding = false;
    



    public enum BoostType { None, Dash, Jump }
    private BoostType currentBoost = BoostType.None;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        bool grounded = IsGrounded();
        bool breaked = IsBreak();
        bool wallleft = IsWalledLeft();
        bool wallright = IsWalledRight();
        RaycastHit2D rightHit = CastDiagonalRayRight();
        RaycastHit2D leftHit = CastDiagonalRayLeft();
        RaycastHit2D breakHit = IsBreak();
        spaceHeld = Input.GetKey(KeyCode.Space);
        bool groundedNow = IsGrounded() || IsBreak();
        bool noInput = !leftHeld && !rightHeld && !spaceHeld;
        IsGrounded();


        bool leftInputDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A);
        bool rightInputDown = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.S);
        bool leftInputHeld = Input.GetMouseButton(0) || Input.GetKey(KeyCode.A);
        bool rightInputHeld = Input.GetMouseButton(1) || Input.GetKey(KeyCode.S);
        bool anyMouseInput = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);

        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isInAir = isJumping || isDashing || isFlying || isBoostFlying;
        bool disablePlatformCollision = isMoving || isInAir;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("OneWayGround"),
            disablePlatformCollision
        );

        if (Input.GetKeyDown(KeyCode.O))
        {
            dirseto = true;
            dir = 1f;
            dirsetofl = 1f;
            groundrayDistance = 1.3f;
            breakrayDistance = 1.4f;
            WallrayDistance = 0.5f;
            raytrens = 0.7f;
            rayLength = 0.8f;

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            dirseto = false;
            dirsetofl = 0.5f;
            dir = 0.5f;
            groundrayDistance = 0.55f;
            breakrayDistance = 0.6f;
            WallrayDistance = 0.25f;
            raytrens = 0.35f;
            rayLength = 0.4f;
        }



        RaycastHit2D hit = IsBreak();

        leftHeld = leftInputHeld;
        rightHeld = rightInputHeld;

        if (leftInputDown)
        {
            if(dirseto == true) dir = -1f;
            else if (dirseto == false) dir = -0.5f;


            if (lastClickDir == InputDirection.Left && Time.time - leftClickTime < doubleClickThreshold && currentBoost != BoostType.None)
            {
                StartBoostFly(currentBoost, Vector2.left); // ★ Boost 비행 시작
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
            if (dirseto == true) dir = 1f;
            else if (dirseto == false) dir = 0.5f;

            if (lastClickDir == InputDirection.Right && Time.time - rightClickTime < doubleClickThreshold && currentBoost != BoostType.None)
            {
                StartBoostFly(currentBoost, Vector2.right); // ★ Boost 비행 시작
                currentBoost = BoostType.None;
                lastClickDir = InputDirection.None;
            }
            else
            {
                lastClickDir = InputDirection.Right;
                rightClickTime = Time.time;
            }
        }

        transform.localScale = new Vector3(dir, dirsetofl, dirsetofl);

        if ((!grounded || !breaked) && Mathf.Abs(leftClickTime - rightClickTime) < bothClickThreshold && !isFlying && !isJumping && !isDashing && Time.time > fallLockUntil)
        {
            StartFall();
            // 이벤트 타일 낙하 시 파괴
            if (breakHit.collider != null && breakHit.collider.CompareTag("Breakable"))
            {
                Destroy(breakHit.collider.gameObject);
                Debug.Log("낙하 중 이벤트 타일 파괴됨!");
            }
        }

        if (!isFlying && !isDashing && !isJumping && (grounded || breaked) && !isBoostFlying)
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

        if (leftFlying && !leftHeld) EndFlying();
        if (rightFlying && !rightHeld) EndFlying(); 

        if (isBoostFlying)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = boostDirection;

        }
        if (isBoostFlying && anyMouseInput && Time.time - boostStartTime > 0.15f)
        {
            StopBoostFly();
            Debug.Log("비행 중 마우스 입력으로 즉시 취소됨");
        }
        if (hit.collider != null && isFallevent == true)
        {
            Debug.Log("트리거 발동!");

        }

        if (breakHit.collider != null && breakHit.collider.CompareTag("Trap"))
        {
            Debug.Log("이벤트 타일(Trap)에 닿아 사망!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isWallJumping)
        {
            wallStickY = float.NaN;
            isWallJumping = false;
            return;
        }

        if (IsWalledRight() && !IsGrounded())
        {
            if (leftInputDown)
            {
                isWallJumping = true;
                StartFlying(Vector2.left);
                return;
            }

            if (float.IsNaN(wallStickY))
                wallStickY = transform.position.y;

            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(rb.position.x, wallStickY);
            transform.position = new Vector3(transform.position.x, wallStickY, transform.position.z);

            if (rightInputDown && !rightHit.collider)
            {
                wallStickY = float.NaN;
                transform.position += new Vector3(1f, 1f, 0f);
            }
        }
        else if (IsWalledLeft() && !IsGrounded())
        {
            if (rightInputDown)
            {
                isWallJumping = true;
                StartFlying(Vector2.right);
                return;
            }

            if (float.IsNaN(wallStickY))
                wallStickY = transform.position.y;

            rb.linearVelocity = Vector2.zero;
            rb.position = new Vector2(rb.position.x, wallStickY);
            transform.position = new Vector3(transform.position.x, wallStickY, transform.position.z);

            if (leftInputDown && !leftHit.collider)
            {
                wallStickY = float.NaN;
                transform.position += new Vector3(-1f, 1f, 0f);
            }
        }
        else
        {
            wallStickY = float.NaN;
        }

   
 


    }

    void FixedUpdate()
    {
        RaycastHit2D hit = IsBreak();
        if (isFlying)
        {
            float holdTime = Mathf.Clamp(Time.time - holdStartTime, 0f, maxHoldTime);
            float t = holdTime / maxHoldTime;

            if (holdTime >= maxHoldTime)
            {
                EndFlying();
                return;
            }

            float x = flyDirection.x * Mathf.Lerp(2f, maxDistance, t);
            float y = jumpArc.Evaluate(t) * arcHeight;
            rb.linearVelocity = new Vector2(x, y);
        }
        else if (!isFlying && !isDashing && !isJumping && (IsGrounded() || IsBreak()))
        {
            
            if (!isBoostFlying)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (leftHeld && !rightHeld && !isFlying && !isBoostFlying)
            {
                StartFlying(Vector2.left);
                leftFlying = true;
            }
            else if (rightHeld && !leftHeld && !isFlying && !isBoostFlying)
            {
                StartFlying(Vector2.right);
                rightFlying = true;
            }
        }

        // ★ Boost 비행 중 → 바닥에 닿으면 종료
        if (isBoostFlying && (IsGrounded() || IsBreak() || IsWalledLeft() || IsWalledRight()))
        {
            StopBoostFly();
        }

        
    }

    void StartFlying(Vector2 direction)
    {
        isFlying = true;
        holdStartTime = Time.time;
        flyDirection = direction.normalized;
        rb.gravityScale = 0f;
    }

    void EndFlying()
    {
        isFlying = false;
        leftFlying = rightFlying = false;
        rb.gravityScale = 2f;

        if ((IsGrounded() || IsBreak()) && !isJumping)
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

    IEnumerator ResumeFlying(Vector2 dir)
    {
        yield return null;
        if (isRepeatQueued && (IsGrounded() || IsBreak()) && !isFlying && !isJumping)
        {

            StartFlying(dir);
            if (dir.x < 0) leftFlying = true;
            else rightFlying = true;
        }
        isRepeatQueued = false;
    }

    void StartFall()
    {
        isFallevent = true;
        isFlying = false;
        rb.gravityScale = 8.5f;
        rb.linearVelocity = Vector2.down * 20f;
        isFallevent = false;
        Debug.Log("낙하");
    }

    // ★ Boost 비행 시작 함수
    void StartBoostFly(BoostType type, Vector2 direction)
    {
        getskill = true;
        isBoostFlying = true;
        boostStartTime = Time.time;
        rb.gravityScale = 0f;
        if (type == BoostType.Dash)
        {
            boostDirection = direction.normalized * dashSpeed;
            dash = true;
            jump = false;
        }
        else if (type == BoostType.Jump)
        {
            boostDirection = Vector2.up * jumpHeight;
            jump = true;
            dash = false;
        }
    }

    // ★ Boost 비행 종료
    void StopBoostFly()
    {
        
        bool leftInputDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A);
        bool rightInputDown = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.S);
        isBoostFlying = false;
        rb.gravityScale = 2f;
        rb.linearVelocity = Vector2.zero;

        StartCoroutine(DelayedGravityRestore(0.2f));
        if (!isFlying && !isDashing && !isBoostFlying && !isJumping && !(IsGrounded() || IsBreak()))
        {
            if (leftInputDown)
            {
                StartFlying(Vector2.left);
                leftFlying = true;
            }
            else if (rightInputDown)
            {
                StartFlying(Vector2.right);
                rightFlying = true;
            }
        }
        Debug.Log("Boost 비행 종료");
    }
    private bool IsLayer(GameObject obj, string layerName)
    {
        return obj.layer == LayerMask.NameToLayer(layerName);
    }

    IEnumerator DelayedGravityRestore(float delay)
    {
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(delay);
        rb.gravityScale = 2f;
    }

    // ★ 벽 충돌 시 Boost 비행 강제 종료
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (IsLayer(collision.gameObject, "Ground"))
        {
            Debug.Log("바닥 접촉");
            StopBoostFly();
        }

        if (!collision.collider.CompareTag("OneWay"))
        {
            if ((isBoostFlying && collision.gameObject.layer == LayerMask.NameToLayer("Wall")) || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                StopBoostFly();
                Debug.Log("벽에 부딪혀 Boost 정지!");
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            Debug.Log("트랩 콜라이더와 충돌 - 사망!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        if (collision.collider.CompareTag("BounceLeftUp"))
        {
            Debug.Log("왼쪽 위로 튕김");

            // Rigidbody2D에 힘을 줘서 튕기게 만들어요!
            Vector2 bounceForce = new Vector2(-12f, 15f); 
            rb.linearVelocity = bounceForce;
        }
        if (collision.collider.CompareTag("BounceRightUp"))
        {
            Debug.Log("오른쪽으로 튕김");

            // Rigidbody2D에 힘을 줘서 튕기게 만들어요!
            Vector2 bounceForce = new Vector2(12f, 15f);
            rb.linearVelocity = bounceForce;
        }

    }

    public void SetBoost(BoostType type)
    {
        currentBoost = type;
    }

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

    public float GetDir()
    {
        return dir;
    }

    public void SetDir(float value)
    {
        dir = value;
    }

    public bool IsGrounded()
    {
        if (isJumping || isDashing || isFlying || isBoostFlying)
            return false;

        float rayDistance = groundrayDistance;

        Vector2 center = transform.position + Vector3.down * 0.2f;
        Vector2 left = center + Vector2.left * 0.2f;
        Vector2 right = center + Vector2.right * 0.2f;

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
        if (isJumping || isDashing || isFlying || isBoostFlying)
        {
            return new RaycastHit2D(); // 비행/점프/대시 중엔 무시
        }
        float rayDistance = breakrayDistance;
        Vector2 origin = transform.position + Vector3.down * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, eventLayer);



        Debug.DrawRay(origin, Vector2.down * rayDistance, hit.collider ? Color.green : Color.red);

        return hit;
    }

    public bool IsWalledRight()
    {
        if (isJumping || isDashing || isFlying || isBoostFlying)
        {
            return false; // 비행/점프/대시 중엔 무시
        }

        float rayDistance = WallrayDistance;
        Vector2 origin = transform.position + Vector3.right * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, rayDistance, groundLayer);

        // 원웨이 플랫폼 무시 처리 (태그 기반)
        if (hit.collider != null && hit.collider.CompareTag("OneWay"))
        {
            return new RaycastHit2D(); // 무시
        }

        Debug.DrawRay(origin, Vector2.right * rayDistance, hit.collider ? Color.green : Color.red);
        return hit.collider != null;
    }
    public bool IsWalledLeft()
    {
        if (isJumping || isDashing || isFlying || isBoostFlying)
        {
            return false; // 비행/점프/대시 중엔 무시
        }

        float rayDistancee = WallrayDistance;
        Vector2 originn = transform.position + Vector3.left * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(originn, Vector2.left, rayDistancee, groundLayer);

        // 원웨이 플랫폼 무시 처리 (태그 기반)
        if (hit.collider != null && hit.collider.CompareTag("OneWay"))
        {
            return new RaycastHit2D(); // 무시
        }

        Debug.DrawRay(originn, Vector2.left * rayDistancee, hit.collider ? Color.green : Color.red);
        return hit.collider != null;
    }

    public RaycastHit2D CastDiagonalRayRight()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0f, raytrens);
        Vector2 direction = new Vector2(1.8f, 1f).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, groundLayer);

        // 원웨이 플랫폼 무시 처리 (태그 기반)
        if (hit.collider != null && hit.collider.CompareTag("OneWay"))
        {
            return new RaycastHit2D(); // 무시
        }

        Debug.DrawRay(origin, direction * rayLength, hit.collider ? Color.green : Color.red);
        return hit;
    }
    public RaycastHit2D CastDiagonalRayLeft()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0f, raytrens);
        Vector2 direction = new Vector2(-1.8f, 1f).normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, groundLayer);

        // 원웨이 플랫폼 무시 처리 (태그 기반)
        if (hit.collider != null && hit.collider.CompareTag("OneWay"))
        {
            return new RaycastHit2D(); // 무시
        }

        Debug.DrawRay(origin, direction * rayLength, hit.collider ? Color.green : Color.red);
        return hit;
    }
}
