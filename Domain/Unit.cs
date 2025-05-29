using System;
using System.Collections;
using Unity.VisualScripting;
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
    protected Coroutine currentAction;
    protected Action OnDefeated;

    public virtual void Start()
    {
        this.unitSystem = ServiceLocator.Get<UnitSystem>();
        OnDefeated += HandleOnDefeated;
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

    public void TakeDamage(int damage)
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

    protected IEnumerator MoveAndAnimate(Animator animator, Vector3 target, string animName, Action onArrive)
    {
        animator.Play(animName);

        while (Vector3.Distance(transform.position, target) > 0.1f)
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
}
