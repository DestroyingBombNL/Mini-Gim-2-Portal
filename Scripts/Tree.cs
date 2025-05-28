using System.Linq;
using UnityEngine;

public class Tree : MonoBehaviour, ITree
{
    [SerializeField] private ETeam team;
    private Scavenger[] scavengers = new Scavenger[2];
    private Fruit[] fruits = new Fruit[100];

    void Start()
    {
        PopulateFruits();
    }
    private void PopulateFruits()
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            fruits[i] = new Fruit("Luminberry", 5);
        }
    }

    public ETeam GetTeam()
    {
        return this.team;
    }

    public void AddScavenger(Scavenger scavenger)
    {
        int filledCount = this.scavengers.Count(s => s != null); 
        this.scavengers[filledCount] = scavenger;
    }

    public void RemoveScavenger(Scavenger scavenger)
    {
        for (int i = 0; i < this.scavengers.Length; i++)
        {
            if (this.scavengers[i] == scavenger)
            {
                this.scavengers[i] = null;
                break;
            }
        }
    }

    public Fruit RemoveFruit(int amount)
    {
        int filledCount = this.fruits.Count(s => s != null);

        if (filledCount == 0)
        {
            Debug.Log("No Fruits Left");
            return null;
        }

        Fruit fruitClone = fruits[filledCount].Clone();
        fruits[filledCount] = null;
        return fruitClone;
    }
}
