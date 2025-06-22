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


    [Header("보스 등장 설정")]

    public int enemiesToKill = 10;
    private int killCount = 0;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    public Transform player;

    private bool bossSpawned = false;


    private void Awake()
    {
        // 싱글 패턴 (다른 스크립트에서 접근할 수 있음)
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
        Time.timeScale = 0f; // 게임 정지
        Debug.Log("Game Over");
        // Game Over UI 띄우기
        gameOverUI.SetActive(true);
    }

    public void StageClear()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.StageClear;
        Time.timeScale = 0f;
        Debug.Log("Stage Clear");
        // Clear UI 띄우기
        stageClearUI.SetActive(true);
    }

    public void RegisterKill()
    {
        killCount++;
        Debug.Log($"적 처치 수: {killCount}/{enemiesToKill}");

        if (!bossSpawned && killCount >= enemiesToKill)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        bossSpawned=true;
        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        //적에게 플레이어 위치 전달(선택)
        BossAI ai = boss.GetComponent<BossAI>();
        if (ai != null)
        {
            ai.player = player;
        }
        Debug.Log("보스 등장");
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
