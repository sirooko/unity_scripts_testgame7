using UnityEngine;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        Playing,
        GameOver,
        StageClear
    }

    public static GameManager Instance;

    public GameState currentState = GameState.Playing;

    public GameObject gameOverUI;
    public GameObject stageClearUI;


    [Header("���� ���� ����")]

    public int enemiesToKill = 10;
    private int killCount = 0;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    public Transform player;

    private bool bossSpawned = false;


    private void Awake()
    {
        // �̱� ���� (�ٸ� ��ũ��Ʈ���� ������ �� ����)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOver;
        Time.timeScale = 0f; // ���� ����
        Debug.Log("Game Over");
        // Game Over UI ����
        gameOverUI.SetActive(true);
    }

    public void StageClear()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.StageClear;
        Time.timeScale = 0f;
        Debug.Log("Stage Clear");
        // Clear UI ����
        stageClearUI.SetActive(true);
    }

    public void RegisterKill()
    {
        killCount++;
        Debug.Log($"�� óġ ��: {killCount}/{enemiesToKill}");

        if (!bossSpawned && killCount >= enemiesToKill)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        bossSpawned=true;
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        //������ �÷��̾� ��ġ ����(����)
        BossAI ai = boss.GetComponent<BossAI>();
        if (ai != null)
        {
            ai.player = player;
        }
        Debug.Log("���� ����");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
