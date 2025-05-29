using System;
using System.Collections;
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
    [SerializeField] protected Animator animator;
    [SerializeField] protected CircleCollider2D circleCollider2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int sortingOffset; //1000
    protected Coroutine currentAction;
    protected Action OnDefeated;
    protected Action OnAttacking;
    protected Action OnMoving;
    protected Action OnSieging;
    protected GameObject targetEnemy;
    protected bool isSieging = false;

    public virtual void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        OnDefeated += HandleOnDefeated;
        SetRange();
        SetSortingOrder();
    }

    protected void SetSortingOrder()
    {
        this.spriteRenderer.sortingOrder = Mathf.RoundToInt(sortingOffset - transform.position.y * 100);
    }

    protected void SetRange()
    {
        circleCollider2D.radius = range;
    }

    public virtual void Update()
    {

    }

    protected void TriggerOnDefeated() => OnDefeated?.Invoke();

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

    public ETeam GetTeam()
    {
        return this.team;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (currentAction != null) StopCoroutine(currentAction);
            TriggerOnDefeated();
        }
    }

    protected void HandleOnDefeated()
    {
        Debug.Log("Unit got Defeated...");
        Destroy(this.gameObject);
    }

    protected IEnumerator MoveAndAnimate(Animator animator, Vector3 target, string animName, Action onArrive, float deviation)
    {
        animator.Play(animName);

        while (Vector3.Distance(transform.position, target) > deviation)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
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

    protected void TriggerAttacking() => OnAttacking?.Invoke();
    protected void TriggerMoving() => OnMoving?.Invoke();
    protected void TriggerSieging() => OnSieging?.Invoke();

    protected virtual void HandleMoving()
    {
        Debug.Log(unitType.ToString() + " is moving...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(this.animator, this.team == ETeam.Ally ? enemyPortalTransform.position : alliedPortalTransform.position, "Moving", TriggerSieging, 0.9f + range));
    }
}
