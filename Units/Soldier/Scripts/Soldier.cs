using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float effectDelay; //0.2f
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public override void Start()
    {
        base.Start();  // Calls Unit.Start()

        OnMoving += HandleOnMoving;
        this.actionSystem.OnEActionChanged += TriggerOnMoving;

        OnAttacking += HandleOnAttacking;
        OnDefending += HandleOnDefending;
        OnSieging += HandleOnSieging;

        //rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;

        TriggerOnMoving(team, this.actionSystem.GetEAction(team));
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
        isSieging = true;
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

        // Determine offset based on facing direction (scale.x)
        float xOffset = transform.localScale.x >= 0f ? 0.6f : -0.6f;

        Vector3 spawnPosition = new Vector3(
            transform.position.x + xOffset,
            transform.position.y + 0.15f,
            transform.position.z
        );

        Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator AttackUnitUntilDestroyed()
    {
        while (enemiesInRange.Count > 0)
        {
            this.animator.Play("Attacking");
            this.audioSystem.PlaySFX(this.audioSystem.GetAudioClipBasedOnName("SoldierAttack"), 0.2f, 1f);
            StartCoroutine(HitEffectDelay(effectDelay));

            float duration = GetAnimationLength(animator, "Attacking");
            yield return new WaitForSeconds(duration);

            if (enemiesInRange.Count == 0)
            {
                break;
            }

            // Deal damage
            IUnit unit = enemiesInRange[0].GetComponent<IUnit>();
            unit.TakeDamage(this.damage);
        }

        // Enemy was destroyed or set to null
        TriggerOnMoving(team, this.actionSystem.GetEAction(team));
    }

    protected void SearchForNewEnemy()
    {
        float detectionRadius = this.boxCollider2D.size.x;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hit in hits)
        {
            IUnit nearbyUnit = hit.GetComponent<IUnit>();
            if (nearbyUnit != null && nearbyUnit.GetTeam() != this.team)
            {
                OnTriggerEnter2D(hit);
                break;
            }
            else
            {
                TriggerOnMoving(team, this.actionSystem.GetEAction(team));
            }
        }
    }

    private IEnumerator AttackBaseUntilDestroyed()
    {
        while (this.unitSystem.getSiegeTransform(team) != null)
        {
            this.animator.Play("Attacking");
            this.audioSystem.PlaySFX(this.audioSystem.GetAudioClipBasedOnName("SoldierAttack"), 0.2f, 1f);
            StartCoroutine(HitEffectDelay(effectDelay));

            float duration = GetAnimationLength(animator, "Attacking");
            yield return new WaitForSeconds(duration);

            unitSystem.TakeDamage(team, this.damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            IUnit otherUnit = other.GetComponent<IUnit>();
            if (otherUnit != null && otherUnit.GetTeam() != this.team)
            {
                enemiesInRange.Add(other.gameObject);

                if (!isSieging)
                {
                    Vector3 scale = transform.localScale;

                    // Flip sprite based on direction
                    bool isToRight = transform.position.x > other.gameObject.transform.position.x;
                    scale.x = isToRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                    transform.localScale = scale;

                    TriggerOnAttacking();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemiesInRange.Remove(other.gameObject);
    }
}