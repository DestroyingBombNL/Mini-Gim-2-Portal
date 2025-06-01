public interface IGameStateSystem
{
    public event System.Action<EGameState> OnGameStateChanged;
    public void SetGameState(EGameState newState);
}
