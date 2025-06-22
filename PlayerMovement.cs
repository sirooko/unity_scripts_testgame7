using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 5f;           // �¿� �̵� �ӵ�
    public float jumpForce = 5f;           // ���� ��
    public LayerMask groundLayer;          // �ٴ� ������ ���̾�

        private bool wasMoving = false;

    private Rigidbody2D rb;                // ������ٵ�
    private Animator animator;             // �ִϸ�����

    private bool isGround = false;         // �ٴڿ� �ִ��� ����

    private int comboStep = 0;             // ���� �޺� �ܰ�
    private float comboTimer = 0f;         // �޺� Ÿ�̸�
    public float comboDelay = 0.6f;        // �޺� ��ȿ �ð�

    private bool isRolling = false;        // ������ �� ����
    private float rollDuration = 0.5f;     // ������ �ð�
    private float rollTimer = 0f;
    private bool isInvincible = false;     // ���� ����

    [Header("����")]
    public AudioSource audioSource;
    public AudioClip footstepClip;


    [Header("���� ��Ʈ�ڽ�")]
    public GameObject attackHitbox;        // ���� ���� ������Ʈ (�ڽ� ������Ʈ�� ����)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // ���� �� ��Ʈ�ڽ� ��Ȱ��ȭ
        if (attackHitbox != null)
            attackHitbox.SetActive(false);

    }

    void Update()
    {

        // groundLayer�� �ٴ�, enemyLayer�� ������ ����
        int combinedLayer = groundLayer | ( 1 << LayerMask.NameToLayer("Enemy"));
        isGround = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, combinedLayer);

        animator.SetBool("isJumping", !isGround); // ���߿� ������ ���� ��

        // �̵� �Է�
        float moveX = Input.GetAxis("Horizontal");

        bool isMoving = Mathf.Abs(moveX) > 0.1f;

        // �߼Ҹ� ó��
        if (isMoving && isGround && !audioSource.isPlaying)
        {
            audioSource.clip = footstepClip;
            audioSource.Play();
        }
        else if (!isMoving || !isGround)
        {
            audioSource.Stop();
        }

        // ���� ���� �ӵ� ������ ä�� �¿� �̵� ����
        float currentY = rb.linearVelocity.y;
        rb.linearVelocity = new Vector2(moveX * moveSpeed, currentY);

        // ���� ó��
        if (moveX > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);

        // �ٴ� ���� (����ĳ��Ʈ�� �Ʒ��� ���� Ȯ��)
        //isGround = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        // ����
        if (Input.GetKeyDown(KeyCode.X) && isGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // �ִϸ��̼�: �޸���
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetBool("isRunning", horizontalSpeed > 0.1f);

        // �޺� ���� Ÿ�̸� ����
        comboTimer += Time.deltaTime;

        // ���� �Է� ó��
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // �޺� ������ ������ �ʱ�ȭ
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
                comboStep = 0; // 3Ÿ �� �ʱ�ȭ
            }

            comboTimer = 0f; // �޺� Ÿ�̸� �ʱ�ȭ
        }

        // ������ �Է�
        if (Input.GetKeyDown(KeyCode.C) && isGround && !isRolling)
        {
            //Debug.Log(" ������ ����");

            isRolling = true;
            rollTimer = 0f;
            isInvincible = true;

            animator.SetTrigger("Roll");

            // �̵� ���� �������� ������
            float rollDirection = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(rollDirection * 8f, rb.linearVelocity.y);

            // �� �浹 ����
            SetIgnoreEnemyCollision(true);

        }

        // ������ ���� ��
        if (isRolling)
        {
            rollTimer += Time.deltaTime;

            if (rollTimer >= rollDuration)
            {
                //Debug.Log(" ������ ����");

                isRolling = false;
                isInvincible = false;

                // �浹 �ٽ� ���
                SetIgnoreEnemyCollision(false);
            }

        }

        //�߶��� ����
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void SetIgnoreEnemyCollision(bool ignore)
    {

        //int playerLayer = LayerMask.NameToLayer("Player");
        //int enemyLayer = LayerMask.NameToLayer("Enemy");

        //Debug.Log($"[�浹 ���� ����] Player �� Enemy | ���� ����: {ignore} (Player: {playerLayer}, Enemy: {enemyLayer})");

        // Enemy ���̾�� 8���̶�� ���� (Ȯ�� �ʿ�)
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), ignore);
    }


    // ���� �ִϸ��̼ǿ��� ȣ��� �Լ� (�ִϸ��̼� �̺�Ʈ�� ����)
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

    // �ð������� ���� Ȯ�� (����׿�)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);
    }

 
}
