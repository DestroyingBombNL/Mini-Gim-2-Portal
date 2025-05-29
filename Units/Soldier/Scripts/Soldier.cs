using System.Collections;
using UnityEngine;

public class Soldier : Unit
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float effectDelay; //0.2f

    public override void Start()
    {
        base.Start();  // Calls Unit.Start()
        OnAttacking += HandleAttacking;
        OnMoving += HandleMoving;
        OnSieging += HandleSieging;

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        TriggerMoving();
    }

    public override void Update()
    {
        base.Update();  // Calls Unit.Update()
    }

    private void HandleAttacking()
    {
        Debug.Log(unitType.ToString() + " is attacking...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(AttackUnitUntilDestroyed());
    }

    private IEnumerator HitEffectDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 spawnPosition = new Vector3(this.transform.position.x  + 0.6f, this.transform.position.y + 0.15f, this.transform.position.z);
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
        TriggerMoving();
    }

    private IEnumerator AttackBaseUntilDestroyed()
    {
        Transform portalTransform = team == ETeam.Ally ? enemyPortalTransform : alliedPortalTransform;

        while (portalTransform != null)
        {
            this.animator.Play("Attacking");
            StartCoroutine(HitEffectDelay(effectDelay));

            IUnitSystem unitSystem = portalTransform.GetComponent<IUnitSystem>();
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

        if (otherUnit != null && otherUnit.GetTeam() != this.team && !isSieging)
        {
            targetEnemy = other.gameObject;
            TriggerAttacking();
        }
    }

    private void HandleSieging()
    {
        Debug.Log(unitType.ToString() + " is sieging...");
        if (currentAction != null) StopCoroutine(currentAction);
        isSieging = true;
        currentAction = StartCoroutine(AttackBaseUntilDestroyed());
    }
}