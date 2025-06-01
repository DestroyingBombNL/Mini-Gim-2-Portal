using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitPrefabEntry
{
    public EUnit unit;
    public GameObject prefab;
}

[System.Serializable]
public class TeamUnitEntry
{
    public ETeam team;
    public int health;
    public Transform spawnerTransform;
    public Transform defendTransform;
    public Transform siegeTransform;
    public Transform unitContainerTransform;
    public UnitPrefabEntry[] unitPrefabEntries;
}

public class UnitSystem : MonoBehaviour, IUnitSystem
{
    public event System.Action<ETeam, float> OnUnitSpawned;
    [SerializeField] private float spawnYOffsetMin; //0.25f
    [SerializeField] private float spawnYOffsetMax; //0.5f
    [SerializeField] private TeamUnitEntry[] teamUnitEntries;
    [SerializeField] private int playerUnitHealthBuff;
    [SerializeField] private Animator animator;
    private Dictionary<ETeam, TeamUnitEntry> teamMap = new();
    private IEnergySystem energySystem;
    private AudioSystem audioSystem;
    private GameStateSystem gameStateSystem;

    void Awake()
    {
        foreach (var entry in teamUnitEntries)
        {
            teamMap[entry.team] = entry;
            if (entry.team == ETeam.Ally)
            {
                teamMap[entry.team].health += playerUnitHealthBuff;
            }
        }
    }

    void Start()
    {
        this.energySystem = ServiceLocator.Get<EnergySystem>();
        this.audioSystem = ServiceLocator.Get<AudioSystem>();
        this.gameStateSystem = ServiceLocator.Get<GameStateSystem>();
    }

    public void TakeDamage(ETeam team, int damage)
    {
        ETeam teamWhichTakesDamage = ETeam.Ally == team ? ETeam.Enemy : ETeam.Ally; //Needs tp be reversed, units give the team they are on as parameter
        teamMap[teamWhichTakesDamage].health -= damage;
        if (teamMap[teamWhichTakesDamage].health <= 0)
        {
            OnDefeated(teamWhichTakesDamage);
        }
    }

    public void MockSpawnUnit(ETeam team, EUnit unitType, Vector3 autoSpawnTransformPosition)
    {
        var entry = teamMap[team];
        GameObject unitGameObject = getUnitGameObject(team, unitType);
        Vector3 spawnPosition = autoSpawnTransformPosition;
        spawnPosition.y += UnityEngine.Random.Range(spawnYOffsetMin, spawnYOffsetMax);
        GameObject instantiatedUnit = Instantiate(unitGameObject, spawnPosition, Quaternion.identity, entry.unitContainerTransform);
        IUnit instantiatedUnitScript = instantiatedUnit.GetComponent<IUnit>();
        instantiatedUnitScript.SetSpawnerTransform(entry.spawnerTransform);
        instantiatedUnitScript.SetDefendTransform(entry.defendTransform);
        instantiatedUnitScript.SetSiegeTransform(entry.siegeTransform);
        instantiatedUnitScript.SetTeam(team);
        instantiatedUnitScript.MultiplyMovementSpeedBy(2);
    }

    public IEnumerator SpawnUnitCoroutine(ETeam team, EUnit unitType)
    {
        var entry = teamMap[team];
        GameObject unitGameObject = getUnitGameObject(team, unitType);

        if (energySystem == null)
        {
            energySystem = ServiceLocator.Get<EnergySystem>();
        }

        if (unitGameObject == null)
        {
            Debug.LogError($"No prefab found for unit type {unitType}");
            yield break;
        }

        IUnit unitScript = unitGameObject.GetComponent<IUnit>();
        if (energySystem.RemoveEnergy(team, unitScript))
        {
            Vector3 spawnPosition = entry.spawnerTransform.position;
            spawnPosition.y += UnityEngine.Random.Range(spawnYOffsetMin, spawnYOffsetMax);

            this.animator.Play("Spawn");

            // ✅ Wait for spawn animation duration
            float duration = GetAnimationLength(animator, "Spawn");
            OnUnitSpawned?.Invoke(team, duration);
            yield return new WaitForSeconds(duration);

            if (team == ETeam.Ally)
            {
                this.audioSystem.PlaySFX(this.audioSystem.GetAudioClipBasedOnName("SpawnSound"), 1f, 0f);
            }

            // 🔄 Return to idle animation
            this.animator.Play("Idle");

            GameObject instantiatedUnit = Instantiate(unitGameObject, spawnPosition, Quaternion.identity, entry.unitContainerTransform);
            IUnit instantiatedUnitScript = instantiatedUnit.GetComponent<IUnit>();
            instantiatedUnitScript.SetSpawnerTransform(entry.spawnerTransform);
            instantiatedUnitScript.SetDefendTransform(entry.defendTransform);
            instantiatedUnitScript.SetSiegeTransform(entry.siegeTransform);
            instantiatedUnitScript.SetTeam(team);
        }
        else
        {
            //Debug.Log("Not enough energy to spawn unit!");
        }
    }

    private float GetAnimationLength(Animator targetAnimator, string animName)
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


    private void OnDefeated(ETeam teamWhoLost)
    {
        if (teamWhoLost == ETeam.Ally)
        {
            this.gameStateSystem.SetGameState(EGameState.Over);
        }
        else
        {
            this.gameStateSystem.SetGameState(EGameState.Victorious);
        }
    }

    public GameObject getUnitGameObject(ETeam team, EUnit unitType)
    {
        var unitEntries = teamMap[team].unitPrefabEntries;

        foreach (var entry in unitEntries)
        {
            if (entry.unit == unitType)
            {
                return entry.prefab;
            }
        }

        return null;
    }

    public Transform getSiegeTransform(ETeam team)
    {
        return teamMap[team].siegeTransform;
    }
}
