using UnityEngine;

/// <summary>
/// ĳ���� ���� �� Ÿ�� ���� ó��
/// </summary>
public class AttackHitbox : MonoBehaviour
{
    public int damage = 20; // �� ������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ������ Health ������Ʈ�� ������ �ִٸ�
        Health targetHealth = collision.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage, transform.position);
        }
    }
}
