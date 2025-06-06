public interface ITree
{
    public void PopulateFruits();
    public ETeam GetTeam();
    public void AddScavenger(Scavenger scavenger);
    public void RemoveScavenger(Scavenger scavenger);
    public Fruit RemoveFruit(int amount);
    public int GetScavengerCurrentCapacity();
    public int GetScavengerMaxCapacity();
}