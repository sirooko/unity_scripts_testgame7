using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // 생성할 적 프리팹
    public Transform player;             // 플레이어 위치 참조
    public float spawnInterval = 3f;     // 몇 초마다 생성할지
    public Transform[] spawnPoints;      // 스폰 위치 배열

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 랜덤 위치에서 적 생성
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPos = spawnPoints[index];

        GameObject enemy = Instantiate(enemyPrefab, spawnPos.position, Quaternion.identity);

        //적에게 플레이어 위치 전달(선택)
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.player = player;
        }
    }
}
