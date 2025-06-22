using TMPro;
using UnityEngine;

/// <summary>
/// 체력 시스템: 플레이어/적이 피해를 받고, 체력이 0이 되면 사망 처리
/// </summary>
public class Health : MonoBehaviour
{
    [Header("기본 설정")]
    public int maxHealth = 100;         // 최대 체력
    private int currentHealth;          // 현재 체력

    public GameObject damageTextPrefab;
    public Canvas worldCanvas; // ← Inspector에서 World Space Canvas 연결


    [Header("무적 상태")]
    public bool isInvincible = false;   // 무적 여부
    public float invincibleTime = 0.5f; // 무적 시간
    private float invincibleTimer = 0f;

    private Rigidbody2D rb;
    public float knockbackForce = 5f;

    //private bool isKnockback = false;
    //private float knockbackDuration = 0.2f;
    //private float knockbackTimer = 0f;

    public PlayerHealthUI playerHealthUI;  // 플레이어 전용 UI용


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

        // 시작 시 최대 체력으로 초기화
        currentHealth = maxHealth;

        if (playerHealthUI != null)
            playerHealthUI.SetHealth(1f);  // 초기화

        // 추가: 월드 스페이스 캔버스 자동 참조
        GameObject canvasObj = GameObject.FindWithTag("WorldCanvas");
        if (canvasObj != null)
            worldCanvas = canvasObj.GetComponent<Canvas>();
    }

    void Update()
    {
        // 무적 타이머 감소
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
    /// 데미지를 받는 함수
    /// </summary>
    public void TakeDamage(int amount, Vector2 attackerPosition)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} 남은 체력: {currentHealth}");

        // 데미지 텍스트 생성
        if (damageTextPrefab != null && worldCanvas != null)
        {
            GameObject text = Instantiate(damageTextPrefab, worldCanvas.transform); // 캔버스에 자식으로 생성
            text.transform.position = transform.position + new Vector3(0, 1.2f, -1);
            text.GetComponent<TextMeshProUGUI>().text = $"-{amount}";
            Destroy(text, 1f);
        }


        // 체력바 UI 업데이트
        if (playerHealthUI != null)
            playerHealthUI.SetHealth((float)currentHealth / maxHealth);

        // 넉백 적용
        Vector2 knockbackDir = (transform.position - (Vector3)attackerPosition).normalized;

        // 넉백이 가능한 AI에게 전달
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            //Debug.Log("넉백 실행됨!");
            enemyAI.TakeKnockback(attackerPosition, knockbackForce);
        }

        // 무적 적용
        //isInvincible = true;
        //invincibleTimer = invincibleTime;

        // 체력이 0 이하가 되면 죽음 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 사망 처리 (애니메이션 등 추가 가능)
    /// </summary>
    void Die()
    {
        // 예시: 오브젝트 제거
        Debug.Log($"{gameObject.name} 사망");
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
    /// 체력 회복 함수 (선택 사항)
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
    /// 현재 체력 반환
    /// </summary>
    public int GetHealth()
    {
        return currentHealth;
    }
}

