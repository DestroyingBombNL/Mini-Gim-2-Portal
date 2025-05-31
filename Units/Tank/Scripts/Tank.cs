using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Unit
{
    private bool isFortified = false;
    private readonly List<GameObject> nearbyEnemies = new();
    private Coroutine fortifyCoroutine;

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
        StartCoroutine(HandleOnMovingCoroutine(team, eAction));
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


    // protected override IEnumerator MoveAndAnimate(Animator animator, Vector3 target, string animName, Action onArrive, float deviation)
    // {
    //     // Cache original Y so we never modify it
    //     float originalY = rb2D.position.y;

    //     Vector3 scale = transform.localScale;

    //     // Flip sprite based on direction
    //     bool isToRight = transform.position.x > target.x;
    //     scale.x = isToRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
    //     transform.localScale = scale;

    //     // Adjust target based on direction and deviation
    //     if (this.actionSystem.GetEAction(team) == EAction.Defend)
    //     {
    //         if (isToRight)
    //         {
    //             target.x -= deviation * 2f;
    //         }
    //         else
    //         {
    //             target.x += deviation * 2f;
    //         }
    //     }

    //     if (this.actionSystem.GetEAction(team) == EAction.Defend)
    //     {
    //         float randomOffset = UnityEngine.Random.Range(-1.5f, 1.5f);
    //         target.x += randomOffset;
    //     }

    //     animator.Play(animName);

    //     // Move via Rigidbody2D so constraints are honored
    //     while (Mathf.Abs(rb2D.position.x - target.x) > deviation)
    //     {
    //         float newX = Mathf.MoveTowards(rb2D.position.x, target.x, speed * Time.deltaTime);
    //         rb2D.MovePosition(new Vector2(newX, originalY));
    //         yield return null;
    //     }

    //     if (this.actionSystem.GetEAction(team) == EAction.Defend)
    //     {
    //         // Unflip sprite
    //         scale.x = Mathf.Abs(scale.x);
    //         transform.localScale = scale;
    //     }

    //     onArrive?.Invoke();
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IUnit otherUnit = other.GetComponent<IUnit>();
        if (otherUnit != null && otherUnit.GetTeam() != this.team)
        {
            if (!nearbyEnemies.Contains(other.gameObject))
                nearbyEnemies.Add(other.gameObject);

            if (!isFortified)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (nearbyEnemies.Contains(other.gameObject))
            nearbyEnemies.Remove(other.gameObject);

        if (nearbyEnemies.Count == 0 && isFortified)
            TriggerOnMoving(team, this.actionSystem.GetEAction(team));
    }

    public override void TakeDamage(int amount)
    {
        int adjustedDamage = isFortified ? Mathf.FloorToInt(amount * 0.5f) : amount;
        base.TakeDamage(adjustedDamage);
    }
}