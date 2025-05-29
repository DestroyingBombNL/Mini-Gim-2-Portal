using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Unit
{
    [SerializeField] private Rigidbody2D rb2D;
    private bool isFortified = false;
    private readonly List<GameObject> nearbyEnemies = new();
    private Action OnFortifyUp;
    private Action OnFortifyDown;

    public override void Start()
    {
        base.Start();

        OnMoving += HandleMoving;
        OnFortifyUp += HandleFortifyUp;
        OnFortifyDown += HandleFortifyDown;

        rb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
        TriggerMoving();
    }

    protected void TriggerFortifyUp() => OnFortifyUp?.Invoke();
    protected void TriggerFortifyDown() => OnFortifyDown?.Invoke();
    private void OnTriggerEnter2D(Collider2D other)
    {
        IUnit otherUnit = other.GetComponent<IUnit>();
        if (otherUnit != null && otherUnit.GetTeam() != this.team)
        {
            if (!nearbyEnemies.Contains(other.gameObject))
                nearbyEnemies.Add(other.gameObject);

            if (!isFortified)
                TriggerFortifyUp();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (nearbyEnemies.Contains(other.gameObject))
            nearbyEnemies.Remove(other.gameObject);

        if (nearbyEnemies.Count == 0 && isFortified)
            TriggerFortifyDown();
    }

    public override void TakeDamage(int amount)
    {
        int adjustedDamage = isFortified ? Mathf.FloorToInt(amount * 0.5f) : amount;
        base.TakeDamage(adjustedDamage);
    }

    protected override void HandleMoving()
    {
        Debug.Log(unitType.ToString() + " is moving...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(this.animator, this.team == ETeam.Ally ? enemyPortalTransform.position : alliedPortalTransform.position, "Moving", TriggerFortifyUp, 0.9f + range));
    }

    private void HandleFortifyUp()
    {
        Debug.Log(unitType.ToString() + " is fortifying up...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayFortifyUpAnimation());
    }
    private void HandleFortifyDown()
    {
        Debug.Log(unitType.ToString() + " is fortifying down...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayFortifyDownAnimation());
    }

    private IEnumerator PlayFortifyUpAnimation()
    {
        animator.Play("FortifyUp");
        yield return new WaitForSeconds(GetAnimationLength(animator, "FortifyUp"));
        animator.Play("Fortified");
        isFortified = true;
    }

    private IEnumerator PlayFortifyDownAnimation()
    {
        animator.Play("FortifyDown");
        yield return new WaitForSeconds(GetAnimationLength(animator, "FortifyDown"));
        isFortified = false;
        TriggerMoving();
    }
}