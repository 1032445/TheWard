using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LogbookPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI pageText;

    public static bool IsAnyLogbookOpen { get; private set; }
    private static LogbookPanel openPanel;
    private LogbookPage[] pages;
    private int currentPageIndex;
    private bool isBeingShown;

    private void Awake()
    {
        if (panelRoot != null && !isBeingShown)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOpen || pages == null || pages.Length <= 1)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            ShowPage(currentPageIndex + 1);
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            ShowPage(currentPageIndex - 1);
        }
    }

    public void Show(LogbookPage[] logbookPages)
    {
        if (logbookPages == null || logbookPages.Length == 0)
        {
            return;
        }

        pages = logbookPages;
        currentPageIndex = 0;
        ShowPage(currentPageIndex);

        if (panelRoot != null)
        {
            isBeingShown = true;
            panelRoot.SetActive(true);
            isBeingShown = false;
        }

        openPanel = this;
        IsAnyLogbookOpen = true;
    }

    private void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0)
        {
            return;
        }

        currentPageIndex = Mathf.Clamp(pageIndex, 0, pages.Length - 1);
        LogbookPage page = pages[currentPageIndex];

        if (titleText != null)
        {
            titleText.text = page.Title;
        }

        if (contentText != null)
        {
            contentText.text = BuildLogbookText(page);
        }

        if (pageText != null)
        {
            pageText.text = $"{currentPageIndex + 1} / {pages.Length}";
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (openPanel == this)
        {
            openPanel = null;
        }

        pages = null;
        currentPageIndex = 0;
        IsAnyLogbookOpen = false;
    }

    public static void HideOpenPanel()
    {
        if (openPanel != null)
        {
            openPanel.Hide();
            return;
        }

        IsAnyLogbookOpen = false;
    }

    private string BuildLogbookText(LogbookPage page)
    {
        StringBuilder builder = new StringBuilder();

        AddLine(builder, "Entry", page.Entry);
        AddLine(builder, "Date", page.Date);
        AddLine(builder, "System", page.System);
        AddLine(builder, "Area", page.Area);
        AddLine(builder, "Dose Cycle", page.DoseCycle);
        AddLine(builder, "Batch", page.Batch);
        AddLine(builder, "Machine Status", page.MachineStatus);
        AddLine(builder, "Facility Response", page.FacilityResponse);

        if (!string.IsNullOrWhiteSpace(page.Notes))
        {
            builder.AppendLine();
            builder.AppendLine(page.Notes);
        }

        return builder.ToString();
    }

    private void AddLine(StringBuilder builder, string label, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        builder.Append(label);
        builder.Append(": ");
        builder.AppendLine(value);
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
