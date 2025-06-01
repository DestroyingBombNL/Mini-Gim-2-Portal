using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatsDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cheatsText;
    
    void Start()
    {
        cheatsText.enabled = ServiceLocator.Get<GameStateSystem>().GetCheatsState();
    }
}
