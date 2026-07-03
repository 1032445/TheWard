using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSequence : MonoBehaviour
{
    [SerializeField] private float delayBeforeFade = 0.5f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float delayAfterFade = 0.5f;
    [SerializeField] private string radioMenuSceneName = "Menu After Radio";

    private bool isPlaying;

    public static bool IsEndingSequencePlaying { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        IsEndingSequencePlaying = false;
    }

    private void Awake()
    {
        IsEndingSequencePlaying = false;
    }

    private void OnDestroy()
    {
        IsEndingSequencePlaying = false;
    }

    public void Play()
    {
        if (isPlaying)
        {
            return;
        }

        StartCoroutine(PlayRoutine());
    }

    public void LoadRadioMenu()
    {
        if (isPlaying)
        {
            return;
        }

        StartCoroutine(LoadRadioMenuRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        isPlaying = true;
        IsEndingSequencePlaying = true;
        CanvasGroup transitionGroup = CreateTransitionOverlay(false);

        if (delayBeforeFade > 0f)
        {
            yield return new WaitForSeconds(delayBeforeFade);
        }

        yield return FadeCanvasGroup(transitionGroup, 0f, 1f, fadeDuration);

        if (delayAfterFade > 0f)
        {
            yield return new WaitForSeconds(delayAfterFade);
        }

        ClearOpenInteractionState();

        IsEndingSequencePlaying = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private IEnumerator LoadRadioMenuRoutine()
    {
        isPlaying = true;
        IsEndingSequencePlaying = true;

        DontDestroyOnLoad(gameObject);
        CanvasGroup transitionGroup = CreateTransitionOverlay(true);

        if (delayBeforeFade > 0f)
        {
            yield return new WaitForSeconds(delayBeforeFade);
        }

        yield return FadeCanvasGroup(transitionGroup, 0f, 1f, fadeDuration);

        ClearOpenInteractionState();
        SceneManager.LoadScene(radioMenuSceneName);

        yield return null;

        if (delayAfterFade > 0f)
        {
            yield return new WaitForSeconds(delayAfterFade);
        }

        yield return FadeCanvasGroup(transitionGroup, 1f, 0f, fadeDuration);

        if (transitionGroup != null)
        {
            Destroy(transitionGroup.gameObject);
        }

        IsEndingSequencePlaying = false;
        Destroy(gameObject);
    }

    private void ClearOpenInteractionState()
    {
        TextPanel.ResetOpenState();
        DialoguePanel.ResetOpenState();
        PersonnelFilePanel.HideOpenPanel();
        LogbookPanel.HideOpenPanel();
        EndingChoicePanel.HideOpenPanel();
        EndingChoiceObject.ResetChoiceState();
    }

    private CanvasGroup CreateTransitionOverlay(bool persistAcrossSceneLoad)
    {
        GameObject canvasObject = new GameObject("Ending Transition Fade");

        if (persistAcrossSceneLoad)
        {
            DontDestroyOnLoad(canvasObject);
        }

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasGroup canvasGroup = canvasObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        GameObject imageObject = new GameObject("Black");
        imageObject.transform.SetParent(canvasObject.transform, false);

        RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.AddComponent<Image>();
        image.color = Color.black;

        return canvasGroup;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null)
        {
            yield break;
        }

        float elapsed = 0f;
        float safeDuration = Mathf.Max(0.01f, duration);
        group.alpha = from;

        while (elapsed < safeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / safeDuration));
            yield return null;
        }

        group.alpha = to;
    }
}
