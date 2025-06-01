using System.Collections;
using UnityEngine;

public class DelayedIntro : MonoBehaviour
{
    [SerializeField] private GameObject textGameObject;
    [SerializeField] private GameObject buttonGameObject;
    [SerializeField] private float delay; //5f
    void Awake()
    {
        // Disable at awake
        textGameObject.SetActive(false);
        buttonGameObject.SetActive(false);
    }

    void Start()
    {
        // Start coroutine to enable after 5 seconds
        StartCoroutine(EnableAfterDelay(delay));
    }

    private IEnumerator EnableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        textGameObject.SetActive(true);
        buttonGameObject.SetActive(true);
    }
}
