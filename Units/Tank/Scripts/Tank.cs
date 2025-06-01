using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Unit
{
    private bool isFortified = false;
    private Coroutine fortifyCoroutine;
    private readonly List<GameObject> nearbyEnemies = new();

    public override void Start()
    {
        base.Start();

        OnMoving += HandleOnMoving;
        this.actionSystem.OnEActionChanged += TriggerOnMoving;

        OnAttacking += HandleOnAttacking;
        OnDefending += HandleOnDefending;
        OnSieging += HandleOnSieging;

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        TriggerOnMoving(team, this.actionSystem.GetEAction(team));
    }

    public override void Update()
    {
        base.Update();
    }

    private void HandleOnAttacking()
    {
        //Debug.Log(unitType.ToString() + " is attacking...");
        if (currentAction != null) StopCoroutine(currentAction);
        fortifyCoroutine = StartCoroutine(FortifyUp());
    }

    private void HandleOnDefending()
    {
        //Debug.Log(unitType.ToString() + " is defending...");
        if (currentAction != null) StopCoroutine(currentAction);
        fortifyCoroutine = StartCoroutine(FortifyUp());
    }

    private void HandleOnSieging()
    {
        //Debug.Log(unitType.ToString() + " is sieging...");
        if (currentAction != null) StopCoroutine(currentAction);
        fortifyCoroutine = StartCoroutine(FortifyUp());
        isSieging = true;
    }

    private IEnumerator FortifyUp()
    {
        //Debug.Log(unitType.ToString() + " is fortifying up...");

        Vector3 scale = transform.localScale;

        // Flip sprite based on direction
        scale.x = team == ETeam.Enemy ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
        
        animator.Play("FortifyUp");
        yield return new WaitForSeconds(GetAnimationLength(animator, "FortifyUp"));
        animator.Play("Fortified");

        isFortified = true;
    }

    private IEnumerator FortifyDown()
    {
        //Debug.Log(unitType.ToString() + " is fortifying down...");

        Vector3 scale = transform.localScale;

        // Flip sprite based on direction
        scale.x = team == ETeam.Enemy ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
        
        animator.Play("FortifyDown");
        yield return new WaitForSeconds(GetAnimationLength(animator, "FortifyDown"));
        animator.Play("Moving");

        isFortified = false;
    }

    protected override void HandleOnMoving(ETeam team, EAction eAction)
    {
        if (this)
        {
            StartCoroutine(HandleOnMovingCoroutine(team, eAction));
        }
    }

    private IEnumerator HandleOnMovingCoroutine(ETeam team, EAction eAction)
    {
        if (team != this.team) yield break;
        // Wait if fortifyCoroutine is running
        if (fortifyCoroutine != null)
        {
            yield return fortifyCoroutine;
        }

        // If fortified, play fortify down and wait
        if (isFortified)
        {
            fortifyCoroutine = StartCoroutine(FortifyDown());
            yield return fortifyCoroutine;
        }

        //Debug.Log(unitType.ToString() + " is moving...");

        if (currentAction != null)
            StopCoroutine(currentAction);

        Action onArrive = eAction == EAction.Siege ? TriggerOnSieging : TriggerOnDefending;
        Transform location = eAction == EAction.Siege ? siegeTransform : defendTransform;

        currentAction = StartCoroutine(MoveAndAnimate(this.animator, location.position, "Moving", onArrive, 0.3f));
        isSieging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            IUnit otherUnit = other.GetComponent<IUnit>();
            if (otherUnit != null && otherUnit.GetTeam() != this.team)
            {
                nearbyEnemies.Add(other.gameObject);

                if (!isFortified && !isSieging)
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
        nearbyEnemies.Remove(other.gameObject);

        if (nearbyEnemies.Count == 0 && isFortified)
            TriggerOnMoving(team, this.actionSystem.GetEAction(team));
    }

    public override void TakeDamage(int amount)
    {
        int adjustedDamage = isFortified ? Mathf.FloorToInt(amount * 0.5f) : amount;
        if (isFortified)
        {
            this.audioSystem.PlaySFX(this.audioSystem.GetAudioClipBasedOnName("TankHit"), 0.2f, 0f);
        }
        base.TakeDamage(adjustedDamage);
    }
}