using UnityEngine;

/// <summary>
/// 캐릭터 공격 시 타격 판정 처리
/// </summary>
public class AttackHitbox : MonoBehaviour
{
    public int damage = 20; // 줄 데미지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 상대방이 Health 컴포넌트를 가지고 있다면
        Health targetHealth = collision.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage, transform.position);
        }
    }
}
