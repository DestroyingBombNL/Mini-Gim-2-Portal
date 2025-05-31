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

    void Start()
    {
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        this.button.onClick.AddListener(() =>
        {
            this.actionSystem.SetEAction(team, command);
        });

        this.actionSystem.OnEActionChanged += HandleIsSiegingChanged;

        StartCoroutine(updateUI(this.actionSystem.GetEAction(team)));
    }

    void Update()
    {

    }

    private void HandleIsSiegingChanged(ETeam team, EAction action)
    {
        StartCoroutine(updateUI(action));
    }


    private IEnumerator updateUI(EAction action)
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
