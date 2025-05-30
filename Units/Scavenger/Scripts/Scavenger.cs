using System;
using System.Collections;
using UnityEngine;

public class Scavenger : Unit
{
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private float transformsYOffSet;
    private EnergySystem energySystem;
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
        this.energySystem = ServiceLocator.Get<EnergySystem>();
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

    protected override void HandleOnDefeated()
    {
        //Debug.Log("Unit got defeated...");
        if (currentAction != null) StopCoroutine(currentAction);
        this.targetTreeScript.RemoveScavenger(this);
        Destroy(this.gameObject);
    }
    
    protected override IEnumerator MoveAndAnimate(Animator animator, Vector3 target, string animName, Action onArrive, float deviation)
    {
        Vector3 scale = transform.localScale;

        // Flip sprite based on direction
        bool isToRight = transform.position.x > target.x;
        scale.x = isToRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // Adjust target based on direction and deviation
        if (isToRight)
        {
            // Retreating (from right to left), stop further back
            target.x -= deviation * 2f;
        }

        animator.Play(animName);


        while (Vector3.Distance(transform.position, target) > deviation)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        // Unflip sprite
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;

        onArrive?.Invoke();
    }
    
    private void TriggerScavenging() => OnScavenging?.Invoke();
    private void TriggerCollecting() => OnCollecting?.Invoke();
    private void TriggerReturning() => OnReturning?.Invoke();
    private void TriggerOffLoading() => OnOffLoading?.Invoke();

    private void HandleScavenging()
    {
        //Debug.Log("Drone is scavenging...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(MoveAndAnimate(animator, new Vector3(targetTreeTransform.position.x, targetTreeTransform.position.y + transformsYOffSet, targetTreeTransform.position.z), "Scavenging", TriggerCollecting, 0.1f));
    }

    private void HandleCollecting()
    {
        // Debug.Log("Drone is collecting...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(PlayAnimationThen(animator, "Collecting", TriggerReturning));

        PlayFruitAnimation("Float");

        IFruit collectedFruit = targetTreeScript.RemoveFruit(1);
        fruitInventory = collectedFruit;
    }

    private void HandleReturning()
    {
        // Debug.Log("Drone is returning...");
        if (currentAction != null) StopCoroutine(currentAction);
        Vector3 modifiedSpawnerTransform = new Vector3(this.spawnerTransform.position.x, this.spawnerTransform.position.y + transformsYOffSet);
        currentAction = StartCoroutine(MoveAndAnimate(animator, modifiedSpawnerTransform, "Returning", TriggerOffLoading, 0.1f));
    }

    private void HandleOffLoading()
    {
        // Debug.Log("Drone is offLoading...");
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

        this.energySystem.AddEnergy(team, fruitInventory);
        this.fruitInventory = null;
    }
}
