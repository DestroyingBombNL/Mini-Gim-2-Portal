using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStateCanvasEntry
{
    public EGameState state;
    public GameObject canvasObject;
}

public class GameStateSystem : MonoBehaviour, IGameStateSystem
{
    public event System.Action<EGameState> OnGameStateChanged;

    [SerializeField] private EGameState initialState = EGameState.Intro;

    [Header("Systems")]
    [SerializeField] private TreeSystem treeSystem;
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private ActionSystem actionSystem;
    [SerializeField] private AudioSystem audioSystem;

    [Header("Scene Objects")]
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject[] deathBoxes;
    [SerializeField] private GameObject[] autoSpawners;
    [SerializeField] private GameObject[] unitParents;
    [SerializeField] private EnemyAISpawner enemyAISpawner;
    [SerializeField] private GameStateCanvasEntry[] canvasEntries;

    [Header("Gameplay")]
    [SerializeField] private MonoBehaviour[] defaultSpawners;

    private Dictionary<EGameState, GameObject> canvasMap = new();
    private EGameState currentState;

    [Header("Cheats")]
    [SerializeField] private Transform alliedSpawnerTransform;
    private bool reinforcements = false;

    private void Awake()
    {
        foreach (var entry in canvasEntries)
        {
            canvasMap[entry.state] = entry.canvasObject;
        }
    }

    private void Start()
    {
        OnGameStateChanged += ApplyGameState;
        SetGameState(initialState);
    }

    public void SetGameState(EGameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    private void ApplyGameState(EGameState state)
    {
        switch (state)
        {
            case EGameState.Intro:
                HandleIntroState();
                break;
            case EGameState.Tutorial:
                HandleTutorialState();
                break;
            case EGameState.Playing:
                HandlePlayingState();
                break;
            case EGameState.Over:
            case EGameState.Victorious:
                HandleEndState(state);
                break;
        }

        ToggleCanvasForState(state);
    }

    private void HandleIntroState()
    {
        Debug.Log("Intro");
        Time.timeScale = 1f;
        audioSystem.PlayIntroSong();
        audioSystem.MuteSoundEffects();
        SetCameraX(15f);
        ToggleGameObjects(deathBoxes, true);
        ToggleGameObjects(autoSpawners, true);
        actionSystem.SetEAction(ETeam.Ally, EAction.Siege);
        actionSystem.SetEAction(ETeam.Enemy, EAction.Siege);
        enemyAISpawner.enabled = false;
        cameraController.enabled = false;
    }

    private void HandleTutorialState()
    {
        Debug.Log("Tutorial");
        Time.timeScale = 0f;
        ToggleGameObjects(autoSpawners, false);
    }

    private void HandlePlayingState()
    {
        Debug.Log("Playing");
        Time.timeScale = 1f;
        ClearChildren(unitParents);
        ToggleGameObjects(autoSpawners, false);
        enemyAISpawner.enabled = true;
        enemyAISpawner.Initialize();
        energySystem.SetEnergy(ETeam.Ally, 50);
        energySystem.SetEnergy(ETeam.Enemy, 50);
        treeSystem.RepopulateAllTreesWithFruit(ETeam.Ally);
        treeSystem.RepopulateAllTreesWithFruit(ETeam.Enemy);
        actionSystem.SetEAction(ETeam.Ally, EAction.Defend);
        actionSystem.SetEAction(ETeam.Enemy, EAction.Defend);
        audioSystem.StartPlaylist();

        foreach (var spawner in defaultSpawners)
        {
            (spawner as IDefaultSpawner)?.SpawnUnit();
        }
        SetCameraX(-4.8f);
        ToggleGameObjects(deathBoxes, false);
        cameraController.enabled = true;
        if (reinforcements == true)
        {
            foreach (GameObject autoSpawner in autoSpawners)
            {
                if (autoSpawner.GetComponent<AutoSpawner>().GetTeam() == ETeam.Ally)
                {
                    autoSpawner.SetActive(true);
                    autoSpawner.gameObject.transform.position = alliedSpawnerTransform.position;
                }
            }
        }
        audioSystem.UnmuteSoundEffects();
    }

    private void HandleEndState(EGameState state)
    {
        Debug.Log(state == EGameState.Over ? "Game Over" : "Victorious");
        Time.timeScale = 0f;
        enemyAISpawner.enabled = false;
        cameraController.enabled = false;
    }

    private void ToggleCanvasForState(EGameState activeState)
    {
        foreach (var entry in canvasMap)
        {
            entry.Value.SetActive(entry.Key == activeState);
        }
    }

    private void SetCameraX(float x)
    {
        var pos = mainCamera.transform.position;
        pos.x = x;
        mainCamera.transform.position = pos;
    }

    private void ToggleGameObjects(GameObject[] gameObjects, bool active)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(active);
        }
    }

    private void ClearChildren(GameObject[] parents)
    {
        foreach (var parent in parents)
        {
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ToggleCheats(bool toggle)
    {
        this.reinforcements = toggle;
    }
    
    public bool GetCheatsState()
    {
        return this.reinforcements;
    }
}
