using System;
using System.Collections;
using UnityEngine;

public class Scavenger : Unit
{
    [SerializeField] Animator animator;
    private ResourceSystem resourceSystem;
    private TreeSystem treeSystem;
    private Transform targetTreeTransform;
    private ITree targetTreeScript;
    private GameObject fruitPrefab;
    private IFruit fruitInventory;
    private Action OnScavenging;
    private Action OnCollecting;
    private Action OnReturning;
    private Action OnOffLoading;
    private Action OnTakingDamage;

    public override void Start()
    {
        base.Start();  // Calls Unit.Start()
        this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
        this.treeSystem = ServiceLocator.Get<TreeSystem>();
        GameObject nearestTree = this.treeSystem.GetNearestTree(team);
        this.targetTreeTransform = nearestTree.transform;
        this.targetTreeScript = nearestTree.GetComponent<ITree>();

        OnScavenging += HandleScavenging;
        OnCollecting += HandleCollecting;
        OnReturning += HandleReturning;
        OnOffLoading += HandleOffLoading;

        TriggerScavenging();
    }

    public override void Update()
    {
        base.Update();  // Calls Unit.Update()
    }

    private void TriggerScavenging() => OnScavenging?.Invoke();
    private void TriggerCollecting() => OnCollecting?.Invoke();
    private void TriggerReturning() => OnReturning?.Invoke();
    private void TriggerOffLoading() => OnReturning?.Invoke();

    private void HandleScavenging()
    {
        Debug.Log("Drone is scavenging...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(targetTreeTransform.position, "Scavenging", TriggerCollecting));
    }

    private void HandleCollecting()
    {
        Debug.Log("Drone is collecting...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayAnimationThen("Collecting", TriggerReturning));
        Vector3 spawnPosition = transform.position + new Vector3(0, -4f, 0);
        Instantiate(fruitPrefab, spawnPosition, Quaternion.identity, this.transform);
        IFruit collectedFruit = targetTreeScript.RemoveFruit(1);
        fruitInventory = collectedFruit;
    }

    private void HandleReturning()
    {
        Debug.Log("Drone is returning...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(this.team == ETeam.Ally ? alliedPortalTransform.position : enemyPortalTransform.position, "Returning", TriggerOffLoading));
    }

    private void HandleOffLoading()
    {
        Debug.Log("Drone is offLoading...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayAnimationThen("OffLoading", TriggerScavenging));
        this.resourceSystem.AddEnergy(fruitInventory);
        this.fruitInventory = null;
    }

    private IEnumerator MoveAndAnimate(Vector3 target, string animName, Action onArrive)
    {
        //animator.Play(animName);

        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        onArrive?.Invoke();
    }

    private IEnumerator PlayAnimationThen(string animName, Action onComplete)
    {
        //animator.Play(animName);

        // Find the clip length
        float duration = GetAnimationLength(animName);
        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();
    }

    private float GetAnimationLength(string animName)
    {
        // AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        // foreach (var clip in clips)
        // {
        //     if (clip.name == animName)
        //         return clip.length;
        // }

        Debug.LogWarning($"Animation '{animName}' not found!");
        return 1f;
    }
}
