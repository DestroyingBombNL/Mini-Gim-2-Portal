using System;
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    //Unity Editor Configurations
    [SerializeField] protected EUnit unitType;
    [SerializeField] protected int health;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float range;
    [SerializeField] protected int energyCost;
    [SerializeField] protected Animator animator;
    [SerializeField] protected BoxCollider2D boxCollider2D;
    [SerializeField] protected Rigidbody2D rb2D;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected int sortingOffset; //1000

    //Set by parent spawner
    protected ETeam team;
    protected Transform spawnerTransform;
    protected Transform defendTransform;
    protected Transform siegeTransform;

    //Services
    protected UnitSystem unitSystem;
    protected ActionSystem actionSystem;
    protected AudioSystem audioSystem;

    //Actions
    protected Action OnDefeated;
    protected Action<ETeam, EAction> OnMoving;
    protected Action OnAttacking;
    protected Action OnDefending;
    protected Action OnSieging;
    protected Coroutine currentAction;

    protected bool isSieging = false;
    protected bool isAttacking = false;

    public virtual void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        this.audioSystem = ServiceLocator.Get<AudioSystem>();
        OnDefeated += HandleOnDefeated;

        SetRange();
        SetSortingOrder();
    }

    public virtual void Update()
    {

    }

    public void MultiplyMovementSpeedBy(int multiplier)
    {
        this.speed *= multiplier;
    }
    
    protected void SetRange()
    {
        float randomizedRange = range * UnityEngine.Random.Range(0.7f, 1.3f);
        this.boxCollider2D.size = new Vector2(randomizedRange, boxCollider2D.size.y);
    }

    protected void SetSortingOrder()
    {
        this.spriteRenderer.sortingOrder = Mathf.RoundToInt(sortingOffset - transform.position.y * 100);
    }

    protected void TriggerOnDefeated() => OnDefeated?.Invoke();
    protected void TriggerOnMoving(ETeam team, EAction eAction) => OnMoving?.Invoke(team, eAction);
    protected void TriggerOnAttacking() => OnAttacking?.Invoke();
    protected void TriggerOnDefending() => OnDefending?.Invoke();
    protected void TriggerOnSieging() => OnSieging?.Invoke();

    protected virtual void HandleOnDefeated()
    {
        //Debug.Log("Unit got defeated...");
        if (currentAction != null) StopCoroutine(currentAction);

        Destroy(this.gameObject);
    }

    protected virtual void HandleOnMoving(ETeam team, EAction eAction)
    {
        if (this)
        {
            if (team != this.team) return;

            StartCoroutine(WaitUntilNotAttackingAndMove(eAction));
        }
    }

    protected IEnumerator WaitUntilNotAttackingAndMove(EAction eAction)
    {
        // Wait until not attacking
        while (isAttacking)
        {
            yield return null;
        }

        // Stop any current movement
        if (currentAction != null) StopCoroutine(currentAction);

        // Determine destination and arrival callback
        Action onArrive = eAction == EAction.Siege ? TriggerOnSieging : TriggerOnDefending;
        Transform location = eAction == EAction.Siege ? siegeTransform : defendTransform;

        // Start movement
        currentAction = StartCoroutine(MoveAndAnimate(this.animator, location.position, "Moving", onArrive, 0.9f + range));
        isSieging = false;
    }


    protected virtual IEnumerator MoveAndAnimate(Animator animator, Vector3 target, string animName, Action onArrive, float deviation)
    {
        // Cache original Y so we never modify it
        float originalY = rb2D.position.y;

        Vector3 scale = transform.localScale;

        // Flip sprite based on direction
        bool isToRight = transform.position.x > target.x;
        scale.x = isToRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Adjust target based on direction and deviation
        if (this.actionSystem.GetEAction(team) == EAction.Defend)
        {
            if (team == ETeam.Ally && isToRight)
            {
                target.x -= deviation * 2f;
            }
            else if (team == ETeam.Enemy && !isToRight)
            {
                target.x += deviation * 2f;
            }
        }

        if (this.actionSystem.GetEAction(team) == EAction.Defend)
        {
            float randomOffset = UnityEngine.Random.Range(-0.25f, 0.25f);
            target.x += randomOffset;
        }

        animator.Play(animName);

        // Move via Rigidbody2D so constraints are honored
        while (Mathf.Abs(rb2D.position.x - target.x) > deviation)
        {
            float newX = Mathf.MoveTowards(rb2D.position.x, target.x, speed * Time.deltaTime);
            rb2D.MovePosition(new Vector2(newX, originalY));
            yield return null;
        }

        if (this.actionSystem.GetEAction(team) == EAction.Defend)
        {
            // Unflip sprite
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        onArrive?.Invoke();
    }

    protected IEnumerator PlayAnimationThen(Animator animator, string animName, Action onComplete)
    {
        animator.Play(animName);

        // Find the clip length
        float duration = GetAnimationLength(animator, animName);
        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();
    }

    protected float GetAnimationLength(Animator targetAnimator, string animName)
    {
        AnimationClip[] clips = targetAnimator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == animName)
                return clip.length;
        }

        Debug.LogWarning($"Animation '{animName}' not found!");
        return 1f;
    }
    
    public int GetEnergyCost()
    {
        return this.energyCost;
    }

    public void SetSiegeTransform(Transform transform)
    {
        this.siegeTransform = transform;
    }
    public void SetDefendTransform(Transform transform)
    {
        this.defendTransform = transform;
    }

    public void SetSpawnerTransform(Transform transform)
    {
        this.spawnerTransform = transform;
    }

    public ETeam GetTeam()
    {
        return this.team;
    }

    public EUnit GetUnitType()
    {
        return this.unitType;
    }

    public void SetTeam(ETeam team)
    {
        this.team = team;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            TriggerOnDefeated();
        }
    }

    public bool GetIsSieging()
    {
        return this.isSieging;
    }
}
