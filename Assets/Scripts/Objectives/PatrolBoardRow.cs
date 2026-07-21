using TMPro;
using UnityEngine;

public class PatrolBoardRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private bool useSeparateStrikeLine;
    [SerializeField] private GameObject strikeLine;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float completedOpacity = 0.35f;
    [SerializeField] private float activeOpacity = 1f;
    [SerializeField] private float futureOpacity = 0.55f;

    public void SetTask(string taskName, bool isCompleted, bool isActive)
    {
        gameObject.SetActive(true);

        if (labelText != null)
        {
            labelText.text = taskName;
            FontStyles fontStyle = FontStyles.Normal;
            if (isActive)
            {
                fontStyle |= FontStyles.Bold;
            }

            if (isCompleted && !useSeparateStrikeLine)
            {
                fontStyle |= FontStyles.Strikethrough;
            }

            labelText.fontStyle = fontStyle;
            SetTextOpacity(GetOpacity(isCompleted, isActive));
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = GetOpacity(isCompleted, isActive);
        }

        if (strikeLine != null)
        {
            strikeLine.SetActive(useSeparateStrikeLine && isCompleted);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetTextOpacity(float opacity)
    {
        if (labelText == null)
        {
            return;
        }

        Color color = labelText.color;
        color.a = opacity;
        labelText.color = color;
    }

    private float GetOpacity(bool isCompleted, bool isActive)
    {
        if (isCompleted)
        {
            return completedOpacity;
        }

        if (isActive)
        {
            return activeOpacity;
        }

        return futureOpacity;
    }
}
