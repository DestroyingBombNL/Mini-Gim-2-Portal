using System;
using System.Collections;
using UnityEngine;

public class Scavenger : Unit
{
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private float transformsYOffSet;
    private ResourceSystem resourceSystem;
    private TreeSystem treeSystem;
    private Transform targetTreeTransform;
    private ITree targetTreeScript;
    private IFruit fruitInventory;
    private Action OnScavenging;
    private Action OnCollecting;
    private Action OnReturning;
    private Action OnOffLoading;

    public override void Start()
    {
        base.Start();  // Calls Unit.Start()
        this.resourceSystem = ServiceLocator.Get<ResourceSystem>();
        this.treeSystem = ServiceLocator.Get<TreeSystem>();
        GameObject nearestTree = this.treeSystem.GetNearestTree(team);
        this.targetTreeTransform = nearestTree.transform;
        this.targetTreeScript = nearestTree.GetComponent<ITree>();
        this.targetTreeScript.AddScavenger(this);

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
    private void TriggerOffLoading() => OnOffLoading?.Invoke();

    private void HandleScavenging()
    {
        Debug.Log("Drone is scavenging...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(animator, new Vector3(targetTreeTransform.position.x, targetTreeTransform.position.y + transformsYOffSet, targetTreeTransform.position.z), "Scavenging", TriggerCollecting, 0.1f));
    }

    private void HandleCollecting()
    {
        Debug.Log("Drone is collecting...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayAnimationThen(animator, "Collecting", TriggerReturning));

        PlayFruitAnimation("Float");

        IFruit collectedFruit = targetTreeScript.RemoveFruit(1);
        fruitInventory = collectedFruit;
    }

    private void HandleReturning()
    {
        Debug.Log("Drone is returning...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(animator, this.team == ETeam.Ally ? new Vector3(alliedPortalTransform.position.x, alliedPortalTransform.position.y + transformsYOffSet, alliedPortalTransform.position.z) : new Vector3(enemyPortalTransform.position.x, enemyPortalTransform.position.y + transformsYOffSet, enemyPortalTransform.position.z), "Returning", TriggerOffLoading, 0.1f));
    }

    private void HandleOffLoading()
    {
        Debug.Log("Drone is offLoading...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayAnimationThen(animator, "OffLoading", TriggerScavenging));

        float floatAnimLength = PlayFruitAnimation("Drop");
        StartCoroutine(HandleFruitConsumption(floatAnimLength));
    }

    private float PlayFruitAnimation(string animName)
    {
        GameObject fruitGameObject = Instantiate(fruitPrefab, this.transform.position, Quaternion.identity, this.transform);
        Animator fruitAnimator = fruitGameObject.GetComponent<Animator>();
        fruitAnimator.Play(animName);
        float floatAnimLength = GetAnimationLength(fruitAnimator, animName);
        Destroy(fruitGameObject, floatAnimLength);
        return floatAnimLength;
    }

    private IEnumerator HandleFruitConsumption(float delay)
    {
        yield return new WaitForSeconds(delay);

        this.resourceSystem.AddEnergy(fruitInventory);
        this.fruitInventory = null;
    }
}
