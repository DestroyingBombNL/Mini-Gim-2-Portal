using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image imageBackground;
    [SerializeField] Image imageLogo;
    [SerializeField] bool command; //True = IsSieging in ActionSystem to be true, vice-versa as well;
    [SerializeField] private float fillDuration; //4f
    private ActionSystem actionSystem;

    void Start()
    {
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        this.button.onClick.AddListener(() =>
        {
            this.actionSystem.SetIsSieging(command);
        });
        this.actionSystem.OnIsSiegingChanged += HandleIsSiegingChanged;

        StartCoroutine(updateUI());
    }

    void Update()
    {

    }

    private void HandleIsSiegingChanged()
    {
        StartCoroutine(updateUI());
    }


    private IEnumerator updateUI()
    {
        button.interactable = false;

        bool isSieging = this.actionSystem.GetIsSieging();

        Color colorBackground = this.imageBackground.color;
        colorBackground.a = isSieging == command ? 1f : 0.5f;
        this.imageBackground.color = colorBackground;

        Color colorLogo = this.imageLogo.color;
        colorLogo.a = isSieging == command ? 1f : 0.5f;
        this.imageLogo.color = colorLogo;

        yield return StartCoroutine(EnableButtonAfterFill());

        this.button.interactable = isSieging == command ? false : true;
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
