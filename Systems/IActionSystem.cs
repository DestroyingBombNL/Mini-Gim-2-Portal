public interface IActionSystem
{
    public event System.Action<ETeam, EAction> OnEActionChanged;
    public EAction GetEAction(ETeam team);
    public void SetEAction(ETeam team, EAction action);
}