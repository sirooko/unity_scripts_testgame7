// EnemyAttackHitbox.cs
using UnityEngine;
using System.Collections.Generic;

public class EnemyAttackHitbox : MonoBehaviour
{

    [Header("Reference")]

    public int Damage =1;

    private HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

    void OnEnable()
    {
        damagedTargets.Clear(); // 공격 한 번마다 초기화
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (damagedTargets.Contains(other.gameObject)) return;

        Health playerHealth = other.GetComponent<Health>();
        if (playerHealth != null && other.CompareTag("Player"))
        {
            playerHealth.TakeDamage(Damage, transform.position);
            damagedTargets.Add(other.gameObject);
        }
    }
}
