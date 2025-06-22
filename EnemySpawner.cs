using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // ������ �� ������
    public Transform player;             // �÷��̾� ��ġ ����
    public float spawnInterval = 3f;     // �� �ʸ��� ��������
    public Transform[] spawnPoints;      // ���� ��ġ �迭

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
        // ���� ��ġ���� �� ����
        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPos = spawnPoints[index];

        GameObject enemy = Instantiate(enemyPrefab, spawnPos.position, Quaternion.identity);

        //������ �÷��̾� ��ġ ����(����)
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.player = player;
        }
    }
}
