using System.Collections;
using UnityEngine;

public class Soldier : Unit
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float effectDelay; //0.2f
    protected GameObject targetEnemy;
    
    public override void Start()
    {
        base.Start();  // Calls Unit.Start()

        OnMoving += HandleOnMoving;
        this.actionSystem.OnIsSiegingChanged += TriggerOnMoving;

        OnAttacking += HandleOnAttacking;
        OnDefending += HandleOnDefending;
        OnSieging += HandleOnSieging;

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        TriggerOnMoving();
    }

    public override void Update()
    {
        base.Update();  // Calls Unit.Update()
    }

    private void HandleOnAttacking()
    {
        //Debug.Log(unitType.ToString() + " is attacking...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(AttackUnitUntilDestroyed());
    }

    private void HandleOnDefending()
    {
        //Debug.Log(unitType.ToString() + " is defending...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(Idle());
    }

    private void HandleOnSieging()
    {
        //Debug.Log(unitType.ToString() + " is sieging...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(AttackBaseUntilDestroyed());
    }

    private IEnumerator Idle()
    {
        this.animator.Play("Idle");
        //float duration = GetAnimationLength(animator, "Idle");
        yield return new WaitForSeconds(60f);
    }

    private IEnumerator HitEffectDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 spawnPosition = new Vector3(this.transform.position.x + 0.6f, this.transform.position.y + 0.15f, this.transform.position.z);
        Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator AttackUnitUntilDestroyed()
    {
        while (targetEnemy != null)
        {
            this.animator.Play("Attacking");

            StartCoroutine(HitEffectDelay(effectDelay));
            // Deal damage
            IUnit unit = targetEnemy.GetComponent<IUnit>();
            if (unit != null)
            {
                unit.TakeDamage(this.damage);
            }
            else
            {
                break;
            }


            float duration = GetAnimationLength(animator, "Attacking");
            yield return new WaitForSeconds(duration);
        }

        // Enemy was destroyed or set to null
        TriggerOnMoving();
    }

    private IEnumerator AttackBaseUntilDestroyed()
    {
        while (this.siegeTransform != null)
        {
            this.animator.Play("Attacking");
            StartCoroutine(HitEffectDelay(effectDelay));

            IUnitSystem unitSystem = siegeTransform.GetComponent<IUnitSystem>();
            if (unitSystem != null)
            {
                unitSystem.TakeDamage(this.damage);
            }
            else
            {
                break;
            }


            float duration = GetAnimationLength(animator, "Attacking");
            yield return new WaitForSeconds(duration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IUnit otherUnit = other.GetComponent<IUnit>();

        if (otherUnit != null && otherUnit.GetTeam() != this.team)
        {
            targetEnemy = other.gameObject;
            TriggerOnAttacking();
        }
    }
}