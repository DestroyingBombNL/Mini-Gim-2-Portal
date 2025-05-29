using System;
using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    protected UnitSystem unitSystem;
    protected Transform alliedPortalTransform;
    protected Transform enemyPortalTransform;
    [SerializeField] protected ETeam team;
    [SerializeField] protected EUnit unitType;
    [SerializeField] protected int health;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    [SerializeField] protected int energyCost;
    protected Coroutine currentAction;
    protected Action OnDefeated;

    public virtual void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        OnDefeated += HandleOnDefeated;
    }

    public virtual void Update()
    {

    }

    private void TriggerOnDefeated() => OnDefeated?.Invoke();

    public int GetEnergyCost()
    {
        return this.energyCost;
    }

    public void SetAlliedPortalTransform(Transform transform)
    {
        this.alliedPortalTransform = transform;
    }

    public void SetEnemyPortalTransform(Transform transform)
    {
        this.enemyPortalTransform = transform;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (currentAction != null) StopCoroutine(currentAction);
            TriggerOnDefeated();
        }
    }

    private void HandleOnDefeated()
    {
        Debug.Log("Rip...");
    }
}
