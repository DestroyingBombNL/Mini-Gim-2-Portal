using System.Collections;
using UnityEngine;

public class Archer : Unit
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float laserSpawnDelay; //0.15f
    protected GameObject targetEnemy;
    public override void Start()
    {
        base.Start();  // Calls Unit.Start()

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
        base.Update();  // Calls Unit.Update()
    }

    private void HandleOnAttacking()
    {
       // Debug.Log(unitType.ToString() + " is attacking...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(AttackUnitUntilDestroyed());
    }

    private void HandleOnDefending()
    {
        //Debug.Log(unitType.ToString() + " is defending...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(Idle());
    }

    private void HandleOnSieging()
    {
        //Debug.Log(unitType.ToString() + " is sieging...");
        if (currentAction != null) StopCoroutine(currentAction);
        currentAction = StartCoroutine(AttackBaseUntilDestroyed());
    }

    private IEnumerator Idle()
    {
        this.animator.Play("Idle");
        //float duration = GetAnimationLength(animator, "Idle");
        yield return new WaitForSeconds(60f);
    }

    private IEnumerator AttackUnitUntilDestroyed()
    {
        while (targetEnemy != null)
        {
            this.animator.Play("Attacking");
            StartCoroutine(SpawnLaserWithDelay(laserSpawnDelay));

            // Deal damage
            IUnit unit = targetEnemy.GetComponent<IUnit>();

            if (unit == null)
            {
                break;
            }

            float duration = GetAnimationLength(animator, "Attacking");
            yield return new WaitForSeconds(duration);
        }

        // Enemy was destroyed or set to null
        TriggerOnMoving();
    }

    private IEnumerator SpawnLaserWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPosition = new Vector3(this.transform.position.x, this.transform.position.y + 0.25f, this.transform.position.z);
        GameObject laserGameObject = Instantiate(laserPrefab, spawnPosition, Quaternion.identity);
        ILaser laserScript = laserGameObject.GetComponent<ILaser>();
        laserScript.SetTeam(this.team);
        laserScript.SetDamage(this.damage);
    }

    private IEnumerator AttackBaseUntilDestroyed()
    {
        while (this.siegeTransform != null)
        {
            this.animator.Play("Attacking");
            StartCoroutine(SpawnLaserWithDelay(laserSpawnDelay));

            float duration = GetAnimationLength(animator, "Attacking");
            yield return new WaitForSeconds(duration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IUnit otherUnit = other.GetComponent<IUnit>();

        if (otherUnit != null && otherUnit.GetTeam() != this.team)
        {
            targetEnemy = other.gameObject;
            TriggerOnAttacking();
        }
    }
}