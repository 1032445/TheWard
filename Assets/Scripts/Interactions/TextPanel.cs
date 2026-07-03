using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI contentText;

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public static bool IsAnyPanelOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ResetOpenState();
    }

    public static void ResetOpenState()
    {
        IsAnyPanelOpen = false;
    }

    public void Show(string content)
    {
        if (contentText != null)
        {
            contentText.text = content;
        }
        panelRoot.SetActive(true);
        IsAnyPanelOpen = true;
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        IsAnyPanelOpen = false;
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
