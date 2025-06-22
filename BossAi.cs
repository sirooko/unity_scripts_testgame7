using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;                // 플레이어의 위치 참조
    public float moveSpeed = 3f;            // 이동 속도
    public float attackRange = 1.5f;        // 공격 범위

    public GameObject attackHitbox;         // 공격 판정 오브젝트
    public float attackPrepareTime = 1.0f;  // 공격 전 준비 시간
    public float attackRecoveryTime = 1.0f; // 공격 후 멈춤 시간

    private Rigidbody2D rb;                 // 리지드바디
    private Animator animator;              // 애니메이터
    private bool isFacingRight = true;      // 캐릭터 방향 플래그

    private bool recentlyAttacked = false; // 공격 직후 재공격 방지
    private float attackCooldown = 2f;      // 공격 쿨타임
    private float attackCooldownTimer = 0f;


    // 상태 정의
    private enum State { Idle, Moving, PreparingAttack, Attacking, Recovering }
    private State currentState = State.Moving;

    private float stateTimer = 0f;          // 상태 전환 타이머

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 공격 히트박스 시작 시 비활성화
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // 공격 중이 아닐 때만 방향을 플레이어 쪽으로 전환
        if (currentState == State.Moving)
        {
            FlipTowardsPlayer();
        }

        if (player == null) return; // 플레이어가 없으면 아무 것도 하지 않음

        float distance = Vector2.Distance(transform.position, player.position); // 플레이어 거리 계산

        // 쿨타임 갱신
        if (recentlyAttacked)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                recentlyAttacked = false;
            }
        }

        switch (currentState)
        {
            case State.Moving:
                animator.SetBool("isMoving", true); // 이동 애니메이션 활성화

                if (distance <= attackRange)
                {
                    // 공격 범위 안에 들어오면 공격 준비로 전환
                    TransitionTo(State.PreparingAttack, attackPrepareTime);
                    animator.SetBool("isMoving", false);
                    rb.velocity = Vector2.zero;

                    recentlyAttacked = true;
                    attackCooldownTimer = attackCooldown; // 쿨타임 시작
                }
                else
                {
                    MoveTowardPlayer(); // 플레이어를 향해 이동
                }
                break;

            case State.PreparingAttack:
                rb.velocity = Vector2.zero; // 공격 전 정지
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    animator.SetTrigger("isAttacking"); // 공격 애니메이션 트리거
                    TransitionTo(State.Attacking, 0.5f); // 공격 지속 시간 설정
                }
                break;

            case State.Attacking:
                rb.velocity = Vector2.zero; // 공격 중 정지
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    TransitionTo(State.Recovering, attackRecoveryTime); // 후딜 시작
                }
                break;

            case State.Recovering:
                rb.velocity = Vector2.zero; // 공격 후 정지
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    TransitionTo(State.Moving); // 이동으로 복귀
                }
                break;

            case State.Idle:
                rb.velocity = Vector2.zero;
                animator.SetBool("isMoving", false);
                break;
        }

        // 보스 낙사 처리
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    // 플레이어를 향해 이동
    void MoveTowardPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // 방향 전환
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
        {
            Flip();
        }
    }

    // 상태 전환 함수
    void TransitionTo(State newState, float duration = 0f)
    {
        currentState = newState;
        stateTimer = duration;
    }

    // 좌우 반전 처리
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = isFacingRight ? 1 : -1;
        transform.localScale = scale;
    }

    // 애니메이션 이벤트에서 호출됨: 공격 시작 시
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    // 애니메이션 이벤트에서 호출됨: 공격 종료 시
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    // 플레이어 방향을 바라볼 수 있게
    void FlipTowardsPlayer()
    {
        if (player == null) return;

        float directionX = player.position.x - transform.position.x;

        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            Flip();
        }
    }

   
}
