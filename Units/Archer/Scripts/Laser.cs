using UnityEngine;

public class Laser : MonoBehaviour, ILaser
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed; //4f
    [SerializeField] private float timeToDespawn; //3f
    [SerializeField] private GameObject hitEffectPrefab;
    private ETeam team;
    private int damage;
    private UnitSystem unitSystem;

    void Start()
    {
        // Move right for Ally, left for Enemy
        rb.linearVelocity = (team == ETeam.Ally ? Vector2.right : Vector2.left) * speed;
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        Destroy(gameObject, timeToDespawn);
    }

    public void SetTeam(ETeam team)
    {
        this.team = team;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Unit
        IUnit unit = other.GetComponent<IUnit>();
        if (unit != null && unit.GetTeam() != this.team)
        {
            unit.TakeDamage(damage);
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            return;
        }

        // Portal
        if (other.transform == unitSystem.getSiegeTransform(team))
        {
            unitSystem.TakeDamage(team, damage);
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject);
        }
    }
}
