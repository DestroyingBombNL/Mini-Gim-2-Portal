using UnityEngine;
using UnityEngine.UI;

public class SetGameStateButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] EGameState gameState;
    private GameStateSystem gameStateSystem;
   
    void Start()
    {
        this.gameStateSystem = ServiceLocator.Get<GameStateSystem>();
        this.button.onClick.AddListener(() =>
        {
            this.gameStateSystem.SetGameState(gameState);
        });
    }

}
