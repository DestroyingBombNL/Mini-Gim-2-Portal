using System;
using System.Collections;
using UnityEngine;

public interface IUnitSystem
{
    public event System.Action<ETeam, float> OnUnitSpawned;
    public IEnumerator SpawnUnitCoroutine(ETeam team, EUnit unitType);
    public void TakeDamage(ETeam team, int damage);
    public GameObject getUnitGameObject(ETeam team, EUnit unitType);
    public Transform getSiegeTransform(ETeam team);
    public void MockSpawnUnit(ETeam team, EUnit unit, Vector3 autoSpawnTransformPosition);
}