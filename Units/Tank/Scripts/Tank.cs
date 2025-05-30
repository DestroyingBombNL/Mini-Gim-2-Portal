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
        this.actionSystem.OnIsSiegingChanged += TriggerOnMoving;

        OnAttacking += HandleOnAttacking;
        OnDefending += HandleOnDefending;
        OnSieging += HandleOnSieging;

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        TriggerOnMoving();
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
    }

    private IEnumerator FortifyUp()
    {
        //Debug.Log(unitType.ToString() + " is fortifying up...");

        animator.Play("FortifyUp");
        yield return new WaitForSeconds(GetAnimationLength(animator, "FortifyUp"));
        animator.Play("Fortified");

        isFortified = true;
    }

    private IEnumerator FortifyDown()
    {
        //Debug.Log(unitType.ToString() + " is fortifying down...");

        animator.Play("FortifyDown");
        yield return new WaitForSeconds(GetAnimationLength(animator, "FortifyDown"));
        animator.Play("Moving");

        isFortified = false;
    }

    protected override void HandleOnMoving()
    {
        StartCoroutine(HandleOnMovingCoroutine());
    }

    private IEnumerator HandleOnMovingCoroutine()
    {
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

        bool isSieging = this.actionSystem.GetIsSieging();
        Action onArrive = isSieging ? TriggerOnSieging : TriggerOnDefending;
        Transform location = isSieging ? siegeTransform : defendTransform;

        currentAction = StartCoroutine(MoveAndAnimate(this.animator, location.position, "Moving", onArrive, 0.3f));
    }


    protected override IEnumerator MoveAndAnimate(Animator animator, Vector3 target, string animName, Action onArrive, float deviation)
    {
        // Cache original Y so we never modify it
        float originalY = rb2D.position.y;

        Vector3 scale = transform.localScale;

        // Flip sprite based on direction
        bool isToRight = transform.position.x > target.x;
        scale.x = isToRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Adjust target based on direction and deviation
        if (isToRight)
            target.x -= deviation * 2f;

        if (!this.actionSystem.GetIsSieging())
        {
            float randomOffset = UnityEngine.Random.Range(-1.5f, 1.5f);
            target.x += randomOffset;
        }

        // Move via Rigidbody2D so constraints are honored
        while (Mathf.Abs(rb2D.position.x - target.x) > deviation)
        {
            float newX = Mathf.MoveTowards(rb2D.position.x, target.x, speed * Time.deltaTime);
            rb2D.MovePosition(new Vector2(newX, originalY));
            yield return null;
        }

        // Unflip sprite
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;

        onArrive?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IUnit otherUnit = other.GetComponent<IUnit>();
        if (otherUnit != null && otherUnit.GetTeam() != this.team)
        {
            if (!nearbyEnemies.Contains(other.gameObject))
                nearbyEnemies.Add(other.gameObject);

            if (!isFortified)
                TriggerOnAttacking();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (nearbyEnemies.Contains(other.gameObject))
            nearbyEnemies.Remove(other.gameObject);

        if (nearbyEnemies.Count == 0 && isFortified)
            TriggerOnMoving();
    }

    public override void TakeDamage(int amount)
    {
        int adjustedDamage = isFortified ? Mathf.FloorToInt(amount * 0.5f) : amount;
        base.TakeDamage(adjustedDamage);
    }
}