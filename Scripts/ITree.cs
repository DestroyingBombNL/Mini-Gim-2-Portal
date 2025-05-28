public interface ITree
{
    public ETeam GetTeam();
    public void AddScavenger(Scavenger scavenger);
    public void RemoveScavenger(Scavenger scavenger);
    public Fruit RemoveFruit(int amount);
}