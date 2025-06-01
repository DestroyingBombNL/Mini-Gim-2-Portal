using UnityEngine;
using UnityEngine.UI;

public class ToggleCheats : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    private GameStateSystem gameStateSystem;

    void Start()
    {
        toggle.isOn = false;
        gameStateSystem = ServiceLocator.Get<GameStateSystem>();

        // Add listener to toggle changes
        toggle.onValueChanged.AddListener(OnToggleChanged);

        // Initialize cheat state with current toggle value
        gameStateSystem.ToggleCheats(toggle.isOn);
    }

    private void OnToggleChanged(bool isOn)
    {
        gameStateSystem.ToggleCheats(toggle.isOn);
    }
}
