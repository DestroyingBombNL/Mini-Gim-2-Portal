using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [SerializeField] ETeam team;
    [SerializeField] Button button;
    [SerializeField] Image imageBackground;
    [SerializeField] Image imageLogo;
    [SerializeField] EAction command;
    [SerializeField] private float fillDuration; //4f
    private ActionSystem actionSystem;
    private AudioSystem audioSystem;

    void Start()
    {
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        this.audioSystem = ServiceLocator.Get<AudioSystem>();

        this.button.onClick.AddListener(() =>
        {
            this.actionSystem.SetEAction(team, command);
            if (command == EAction.Siege)
            {
                this.audioSystem.PlaySFX(this.audioSystem.GetAudioClipBasedOnName("SiegeCommand"), 1f, 0f);
            }
            else if (command == EAction.Defend)
            {
                this.audioSystem.PlaySFX(this.audioSystem.GetAudioClipBasedOnName("DefendCommand"), 1f, 0f);
            }
        });

        this.actionSystem.OnEActionChanged += HandleIsSiegingChanged;

        StartCoroutine(updateUI(team, this.actionSystem.GetEAction(team)));
    }

    void Update()
    {

    }

    private void HandleIsSiegingChanged(ETeam team, EAction action)
    {
        if (this)
        {
            StartCoroutine(updateUI(team, action)); 
        }
    }


    private IEnumerator updateUI(ETeam team, EAction action)
    {
        if (this.team == team)
        {
            button.interactable = false;
            Color colorBackground = this.imageBackground.color;
            colorBackground.a = action == command ? 1f : 0.5f;
            this.imageBackground.color = colorBackground;

            Color colorLogo = this.imageLogo.color;
            colorLogo.a = action == command ? 1f : 0.5f;
            this.imageLogo.color = colorLogo;

            yield return StartCoroutine(EnableButtonAfterFill());

            this.button.interactable = action == command ? false : true;
        }
        yield break;
    }

    private IEnumerator EnableButtonAfterFill()
    {
        float timer = 0f;
        imageBackground.fillAmount = 0f;

        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            imageBackground.fillAmount = timer / fillDuration;
            yield return null;
        }

        imageBackground.fillAmount = 1f;
    }
}
