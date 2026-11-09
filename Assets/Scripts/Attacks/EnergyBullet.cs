using UnityEngine;

public class EnergyBullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] private float minKnockbackPower;
    [SerializeField] private float maxKnockbackPower;
    [SerializeField] private float minKnockbackUpPower;
    [SerializeField] private float maxKnockbackUpPower;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    private float chargeRate;
    private int ownerId;
    private Vector3 moveDirection;
    private float damage;
    private float knockbackPower;
    private float knockbackUpPower;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime; 
    }
    public void Initialize(float rate, int id, Vector3 direction)
    {
        this.chargeRate = rate;
        this.ownerId = id;
        moveDirection = direction.normalized;

        float size = Mathf.Lerp(minSize, maxSize, chargeRate);
        damage = Mathf.Lerp(minDamage, maxDamage, chargeRate);
        knockbackPower = Mathf.Lerp(minKnockbackPower, maxKnockbackPower, chargeRate);
        knockbackUpPower = Mathf.Lerp(minKnockbackUpPower, maxKnockbackUpPower, chargeRate);
        transform.localScale = Vector3.one * size;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController targetController = other.GetComponentInParent<PlayerController>();
        PlayerStatus targetStatus = other.GetComponentInParent<PlayerStatus>();

        if (targetController == null || targetStatus == null)
        {
            Destroy(gameObject);
            return;
        }
        if (targetStatus.PlayerId == ownerId)
        {
            return;
        }

        targetController.StartKnockback(transform.position, knockbackPower, knockbackUpPower);
        targetStatus.Damage(damage);

        Destroy(gameObject);
    }
}
