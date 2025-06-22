using TMPro;
using UnityEngine;

/// <summary>
/// ü�� �ý���: �÷��̾�/���� ���ظ� �ް�, ü���� 0�� �Ǹ� ��� ó��
/// </summary>
public class Health : MonoBehaviour
{
    [Header("�⺻ ����")]
    public int maxHealth = 100;         // �ִ� ü��
    private int currentHealth;          // ���� ü��

    public GameObject damageTextPrefab;
    public Canvas worldCanvas; // �� Inspector���� World Space Canvas ����


    [Header("���� ����")]
    public bool isInvincible = false;   // ���� ����
    public float invincibleTime = 0.5f; // ���� �ð�
    private float invincibleTimer = 0f;

    private Rigidbody2D rb;
    public float knockbackForce = 5f;

    //private bool isKnockback = false;
    //private float knockbackDuration = 0.2f;
    //private float knockbackTimer = 0f;

    public PlayerHealthUI playerHealthUI;  // �÷��̾� ���� UI��


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

        // ���� �� �ִ� ü������ �ʱ�ȭ
        currentHealth = maxHealth;

        if (playerHealthUI != null)
            playerHealthUI.SetHealth(1f);  // �ʱ�ȭ

        // �߰�: ���� �����̽� ĵ���� �ڵ� ����
        GameObject canvasObj = GameObject.FindWithTag("WorldCanvas");
        if (canvasObj != null)
            worldCanvas = canvasObj.GetComponent<Canvas>();
    }

    void Update()
    {
        // ���� Ÿ�̸� ����
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    /// <summary>
    /// �������� �޴� �Լ�
    /// </summary>
    public void TakeDamage(int amount, Vector2 attackerPosition)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} ���� ü��: {currentHealth}");

        // ������ �ؽ�Ʈ ����
        if (damageTextPrefab != null && worldCanvas != null)
        {
            GameObject text = Instantiate(damageTextPrefab, worldCanvas.transform); // ĵ������ �ڽ����� ����
            text.transform.position = transform.position + new Vector3(0, 1.2f, -1);
            text.GetComponent<TextMeshProUGUI>().text = $"-{amount}";
            Destroy(text, 1f);
        }


        // ü�¹� UI ������Ʈ
        if (playerHealthUI != null)
            playerHealthUI.SetHealth((float)currentHealth / maxHealth);

        // �˹� ����
        Vector2 knockbackDir = (transform.position - (Vector3)attackerPosition).normalized;

        // �˹��� ������ AI���� ����
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            //Debug.Log("�˹� �����!");
            enemyAI.TakeKnockback(attackerPosition, knockbackForce);
        }

        // ���� ����
        //isInvincible = true;
        //invincibleTimer = invincibleTime;

        // ü���� 0 ���ϰ� �Ǹ� ���� ó��
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ��� ó�� (�ִϸ��̼� �� �߰� ����)
    /// </summary>
    void Die()
    {
        // ����: ������Ʈ ����
        Debug.Log($"{gameObject.name} ���");
        GameManager.Instance?.RegisterKill();

        Destroy(gameObject);

        if (CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }

        if (CompareTag("Boss"))
        {
            GameManager.Instance.StageClear();
        }

    }

    /// <summary>
    /// ü�� ȸ�� �Լ� (���� ����)
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    /// <summary>
    /// ���� ü�� ��ȯ
    /// </summary>
    public int GetHealth()
    {
        return currentHealth;
    }
}

