using System.Linq;
using UnityEngine;

public class Tree : MonoBehaviour, ITree
{
    [SerializeField] private ETeam team;
    [SerializeField] private int fruitEnergyAmount;
    private Scavenger[] scavengers = new Scavenger[2];
    private Fruit[] fruits = new Fruit[100];
    private TreeSystem treeSystem;

    void Start()
    {
        this.treeSystem = ServiceLocator.Get<TreeSystem>();
        PopulateFruits();
    }
    public void PopulateFruits()
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            fruits[i] = new Fruit("Luminberry", fruitEnergyAmount);
        }
    }

    public ETeam GetTeam()
    {
        return this.team;
    }

    public void AddScavenger(Scavenger scavenger)
    {
        int scavengerCapacity = GetScavengerCurrentCapacity();
        int index = scavengerCapacity;
        this.scavengers[index] = scavenger;
        this.treeSystem.ReduceScavengerSpotsAvailable(team, 1);
    }

    public void RemoveScavenger(Scavenger scavenger)
    {
        for (int i = 0; i < this.scavengers.Length; i++)
        {
            if (this.scavengers[i] == scavenger)
            {
                this.scavengers[i] = null;
                this.treeSystem.IncreaseScavengerSpotsAvailable(team, 1);
                break;
            }
        }
    }

    public Fruit RemoveFruit(int amount)
    {
        int fruitCapacity = this.fruits.Count(s => s != null);

        if (fruitCapacity == 0)
        {
            Debug.Log("No Fruits Left");
            return null;
        }

        int index = fruitCapacity - 1;

        Fruit fruitClone = fruits[index].Clone();
        fruits[index] = null;
        return fruitClone;
    }

    public int GetScavengerCurrentCapacity()
    {
        return this.scavengers.Count(s => s != null);
    }
    
    public int GetScavengerMaxCapacity()
    {
        return this.scavengers.Length;
    }
}
