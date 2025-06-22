using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform player;                // �÷��̾��� ��ġ ����
    public float moveSpeed = 3f;            // �̵� �ӵ�
    public float attackRange = 1.5f;        // ���� ����

    public GameObject attackHitbox;         // ���� ���� ������Ʈ
    public float attackPrepareTime = 1.0f;  // ���� �� �غ� �ð�
    public float attackRecoveryTime = 1.0f; // ���� �� ���� �ð�

    private Rigidbody2D rb;                 // ������ٵ�
    private Animator animator;              // �ִϸ�����
    private bool isFacingRight = true;      // ĳ���� ���� �÷���

    private bool recentlyAttacked = false; // ���� ���� ����� ����
    private float attackCooldown = 2f;      // ���� ��Ÿ��
    private float attackCooldownTimer = 0f;


    // ���� ����
    private enum State { Idle, Moving, PreparingAttack, Attacking, Recovering }
    private State currentState = State.Moving;

    private float stateTimer = 0f;          // ���� ��ȯ Ÿ�̸�

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // ���� ��Ʈ�ڽ� ���� �� ��Ȱ��ȭ
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // ���� ���� �ƴ� ���� ������ �÷��̾� ������ ��ȯ
        if (currentState == State.Moving)
        {
            FlipTowardsPlayer();
        }

        if (player == null) return; // �÷��̾ ������ �ƹ� �͵� ���� ����

        float distance = Vector2.Distance(transform.position, player.position); // �÷��̾� �Ÿ� ���

        // ��Ÿ�� ����
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
                animator.SetBool("isMoving", true); // �̵� �ִϸ��̼� Ȱ��ȭ

                if (distance <= attackRange)
                {
                    // ���� ���� �ȿ� ������ ���� �غ�� ��ȯ
                    TransitionTo(State.PreparingAttack, attackPrepareTime);
                    animator.SetBool("isMoving", false);
                    rb.velocity = Vector2.zero;

                    recentlyAttacked = true;
                    attackCooldownTimer = attackCooldown; // ��Ÿ�� ����
                }
                else
                {
                    MoveTowardPlayer(); // �÷��̾ ���� �̵�
                }
                break;

            case State.PreparingAttack:
                rb.velocity = Vector2.zero; // ���� �� ����
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    animator.SetTrigger("isAttacking"); // ���� �ִϸ��̼� Ʈ����
                    TransitionTo(State.Attacking, 0.5f); // ���� ���� �ð� ����
                }
                break;

            case State.Attacking:
                rb.velocity = Vector2.zero; // ���� �� ����
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    TransitionTo(State.Recovering, attackRecoveryTime); // �ĵ� ����
                }
                break;

            case State.Recovering:
                rb.velocity = Vector2.zero; // ���� �� ����
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    TransitionTo(State.Moving); // �̵����� ����
                }
                break;

            case State.Idle:
                rb.velocity = Vector2.zero;
                animator.SetBool("isMoving", false);
                break;
        }

        // ���� ���� ó��
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    // �÷��̾ ���� �̵�
    void MoveTowardPlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // ���� ��ȯ
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
        {
            Flip();
        }
    }

    // ���� ��ȯ �Լ�
    void TransitionTo(State newState, float duration = 0f)
    {
        currentState = newState;
        stateTimer = duration;
    }

    // �¿� ���� ó��
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = isFacingRight ? 1 : -1;
        transform.localScale = scale;
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ���: ���� ���� ��
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ���: ���� ���� ��
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    // �÷��̾� ������ �ٶ� �� �ְ�
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
