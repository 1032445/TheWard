using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    private static TextPanel openPanel;
    private string[] pages;
    private int currentPageIndex;

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
        openPanel = null;
        IsAnyPanelOpen = false;
    }

    public static void HideOpenPanel()
    {
        if (openPanel != null)
        {
            openPanel.Hide();
            return;
        }

        IsAnyPanelOpen = false;
    }

    public void Show(string content)
    {
        Show("", content);
    }

    public void Show(string title, string content)
    {
        Show(title, new[] { content });
    }

    public void Show(string title, string[] contentPages)
    {
        gameObject.SetActive(true);

        pages = contentPages != null && contentPages.Length > 0 ? contentPages : new[] { "" };
        currentPageIndex = 0;

        ShowTitle(title);
        ShowCurrentPage();

        panelRoot.SetActive(true);
        openPanel = this;
        IsAnyPanelOpen = true;
    }

    public static bool AdvanceOpenPanel()
    {
        if (openPanel == null)
        {
            IsAnyPanelOpen = false;
            return false;
        }

        return openPanel.Advance();
    }

    public bool Advance()
    {
        if (!IsOpen)
        {
            return false;
        }

        currentPageIndex++;

        if (pages == null || currentPageIndex >= pages.Length)
        {
            Hide();
            return false;
        }

        ShowCurrentPage();
        return true;
    }

    private void ShowTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
            titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
        }
    }

    private void ShowCurrentPage()
    {
        if (contentText != null)
        {
            contentText.text = pages[currentPageIndex];
        }
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        pages = null;
        currentPageIndex = 0;

        if (openPanel == this)
        {
            openPanel = null;
        }

        IsAnyPanelOpen = false;
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
