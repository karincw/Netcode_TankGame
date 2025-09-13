using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int _damage = 10;

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    //����ٰ� ���� �ε����� �� �ش� ������Ʈ�� Health�� �޾ƴٰ�
    //������ TakeDamage�� ȣ���ϰ� �غ���.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody is null) return;


        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_damage);
        }
    }
}
