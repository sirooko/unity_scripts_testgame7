using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;
    public float wallCheckDistance = 0.3f;

    public GameObject attackHitbox;

    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isGrounded = false;

    private float jumpCooldown = 0.2f;
    private float jumpTimer = 0f;

    private Animator animator;

    private bool isAttacking = false;  // 공격 중일 때 움직임 정지용
    private float attackPauseDuration = 1f;  // 정지 시간
    private float attackPauseTimer = 0f;

    private bool isKnockback = false;
    private float knockbackDuration = 0.2f;
    private float knockbackTimer = 0f;



    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 시작 시 히트박스 비활성화
        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        //// 방어적 코드: 플레이어에 붙으면 제거
        //if (CompareTag("Player"))
        //{
        //    Debug.LogWarning("EnemyAI가 플레이어에 붙어있습니다! 제거합니다.");
        //    Destroy(this);
        //}
    }

    public void TakeKnockback(Vector2 attackerPosition, float force)
    {
        //Debug.Log("넉백 적용 함수 내부 진입");

        Vector2 knockbackDir = (transform.position - (Vector3)attackerPosition).normalized;
        rb.velocity = new Vector2(knockbackDir.x * force, rb.velocity.y);

        isKnockback = true;
        knockbackTimer = knockbackDuration;

        //Debug.Log("넉백 적용됨: " + rb.velocity);
    }

    void FixedUpdate()
    {
        // 땅 체크 결과를 저장
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        //Debug.Log("isGrounded = " + isGrounded);
    }

    void Update()
    {
        // 넉백 중이면 이동하지 않음
        if (isKnockback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockback = false;
            }
            return; // 아래 코드 실행 안 함
        }

        // 이동 애니메이션 제어
        animator.SetBool("isMoving", Mathf.Abs(rb.velocity.x) > 0.1f);

        // 점프 애니메이션
        animator.SetBool("isJumping", !isGrounded);

        // 공격
        if (Vector2.Distance(transform.position, player.position) < 0.8f && !isAttacking)
        {
            animator.SetTrigger("isAttacking");
            isAttacking = true;
            attackPauseTimer = attackPauseDuration;
        }

        if (isAttacking)
        {
            attackPauseTimer -= Time.deltaTime;
            rb.velocity = new Vector2(0, rb.velocity.y);  // 이동 멈춤

            if (attackPauseTimer <= 0f)
            {
                isAttacking = false;
            }

            return;  // 공격 중엔 나머지 AI 로직 실행 안함
        }


        if (player == null) return;

        jumpTimer -= Time.deltaTime;

        float directionX = player.position.x - transform.position.x;
        float moveDir = Mathf.Sign(directionX);

        // 이동
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        // 방향 전환
        if ((moveDir > 0 && !isFacingRight) || (moveDir < 0 && isFacingRight))
        {
            Flip();
        }

        // 벽 앞 체크
        Vector2 wallDir = rb.velocity.x > 0 ? Vector2.right : Vector2.left;
        bool isBlocked = Physics2D.Raycast(wallCheck.position, wallDir, wallCheckDistance, groundLayer);

        // 낭떠러지 체크
        Vector2 edgeCheckOrigin = groundCheck.position + (isFacingRight ? Vector3.right : Vector3.left) * 0.5f;
        bool noGroundAhead = !Physics2D.Raycast(edgeCheckOrigin, Vector2.down, 0.2f, groundLayer);

        // 점프 조건
        if (isGrounded && jumpTimer <= 0f && (isBlocked || noGroundAhead))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimer = jumpCooldown;
        }

        // 디버그 라인
        //Debug.DrawRay(wallCheck.position, wallDir * wallCheckDistance, Color.red);
        //Debug.DrawRay(edgeCheckOrigin, Vector2.down * 0.2f, Color.cyan);
        //Debug.DrawRay(groundCheck.position, Vector2.down * checkRadius, Color.green);

        //추락시 제거
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }

    }


    
    public void EnableAttackHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.SetActive(false);
    }


    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = isFacingRight ? 1f : -1f;
        transform.localScale = scale;
    }
}
