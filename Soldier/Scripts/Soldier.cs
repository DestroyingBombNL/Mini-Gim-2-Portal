using System;
using System.Collections;
using UnityEngine;

public class Soldier : Unit
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2D;
    private Action OnAttacking;
    private Action OnMoving;
    private Action OnSieging;
    private GameObject targetEnemy;
    private bool isSieging = false;

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

    private void TriggerAttacking() => OnAttacking?.Invoke();
    private void TriggerMoving() => OnMoving?.Invoke();
    private void TriggerSieging() => OnSieging?.Invoke();

    private void HandleMoving()
    {
        Debug.Log("Soldier is moving...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(animator, this.team == ETeam.Ally ? enemyPortalTransform.position : alliedPortalTransform.position, "Moving", TriggerSieging));
    }

    private void HandleAttacking()
    {
        Debug.Log("Soldier is attacking...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(AttackUnitUntilDestroyed());
    }

    private void HandleSieging()
    {
        Debug.Log("Soldier is sieging...");
        if (currentAction != null) StopCoroutine(currentAction);
        isSieging = true;
        currentAction = StartCoroutine(AttackBaseUntilDestroyed());
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

    private IEnumerator AttackUnitUntilDestroyed()
    {
        while (targetEnemy != null)
        {
            animator.Play("Attacking");

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
            animator.Play("Attacking");

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
}
