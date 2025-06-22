using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;           // 좌우 이동 속도
    public float jumpForce = 5f;           // 점프 힘
    public LayerMask groundLayer;          // 바닥 판정용 레이어

        private bool wasMoving = false;

    private Rigidbody2D rb;                // 리지드바디
    private Animator animator;             // 애니메이터

    private bool isGround = false;         // 바닥에 있는지 여부

    private int comboStep = 0;             // 공격 콤보 단계
    private float comboTimer = 0f;         // 콤보 타이머
    public float comboDelay = 0.6f;        // 콤보 유효 시간

    private bool isRolling = false;        // 구르기 중 여부
    private float rollDuration = 0.5f;     // 구르기 시간
    private float rollTimer = 0f;
    private bool isInvincible = false;     // 무적 여부

    [Header("사운드")]
    public AudioSource audioSource;
    public AudioClip footstepClip;


    [Header("공격 히트박스")]
    public GameObject attackHitbox;        // 공격 판정 오브젝트 (자식 오브젝트로 생성)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 시작 시 히트박스 비활성화
        if (attackHitbox != null)
            attackHitbox.SetActive(false);

    }

    void Update()
    {

        // groundLayer는 바닥, enemyLayer는 적으로 가정
        int combinedLayer = groundLayer | ( 1 << LayerMask.NameToLayer("Enemy"));
        isGround = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, combinedLayer);

        animator.SetBool("isJumping", !isGround); // 공중에 있으면 점프 중

        // 이동 입력
        float moveX = Input.GetAxis("Horizontal");

        bool isMoving = Mathf.Abs(moveX) > 0.1f;

        // 발소리 처리
        if (isMoving && isGround && !audioSource.isPlaying)
        {
            audioSource.clip = footstepClip;
            audioSource.Play();
        }
        else if (!isMoving || !isGround)
        {
            audioSource.Stop();
        }

        // 현재 수직 속도 유지한 채로 좌우 이동 적용
        float currentY = rb.linearVelocity.y;
        rb.linearVelocity = new Vector2(moveX * moveSpeed, currentY);

        // 반전 처리
        if (moveX > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);

        // 바닥 판정 (레이캐스트를 아래로 쏴서 확인)
        //isGround = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        // 점프
        if (Input.GetKeyDown(KeyCode.X) && isGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 애니메이션: 달리기
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetBool("isRunning", horizontalSpeed > 0.1f);

        // 콤보 공격 타이머 갱신
        comboTimer += Time.deltaTime;

        // 공격 입력 처리
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 콤보 딜레이 지나면 초기화
            if (comboTimer > comboDelay)
                comboStep = 0;

            comboStep++;

            if (comboStep == 1)
            {
                animator.SetTrigger("Attack1");
                animator.SetInteger("comboStep", 1);
            }
            else if (comboStep == 2)
            {
                animator.SetTrigger("Attack2");
                animator.SetInteger("comboStep", 2);
            }
            else if (comboStep == 3)
            {
                animator.SetTrigger("Attack3");
                animator.SetInteger("comboStep", 3);
                comboStep = 0; // 3타 후 초기화
            }

            comboTimer = 0f; // 콤보 타이머 초기화
        }

        // 구르기 입력
        if (Input.GetKeyDown(KeyCode.C) && isGround && !isRolling)
        {
            //Debug.Log(" 구르기 시작");

            isRolling = true;
            rollTimer = 0f;
            isInvincible = true;

            animator.SetTrigger("Roll");

            // 이동 방향 기준으로 굴리기
            float rollDirection = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(rollDirection * 8f, rb.linearVelocity.y);

            // 적 충돌 무시
            SetIgnoreEnemyCollision(true);

        }

        // 구르기 중일 때
        if (isRolling)
        {
            rollTimer += Time.deltaTime;

            if (rollTimer >= rollDuration)
            {
                //Debug.Log(" 구르기 종료");

                isRolling = false;
                isInvincible = false;

                // 충돌 다시 허용
                SetIgnoreEnemyCollision(false);
            }

        }

        //추락시 제거
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void SetIgnoreEnemyCollision(bool ignore)
    {

        //int playerLayer = LayerMask.NameToLayer("Player");
        //int enemyLayer = LayerMask.NameToLayer("Enemy");

        //Debug.Log($"[충돌 무시 설정] Player ↔ Enemy | 무시 여부: {ignore} (Player: {playerLayer}, Enemy: {enemyLayer})");

        // Enemy 레이어는 8번이라고 가정 (확인 필요)
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), ignore);
    }


    // 공격 애니메이션에서 호출될 함수 (애니메이션 이벤트로 설정)
    public void EnableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    // 시각적으로 레이 확인 (디버그용)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);
    }

 
}
