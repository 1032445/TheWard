using TMPro;
using UnityEngine;

public class PatrolBoardPanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI completeText;
    [SerializeField] private string title = "NIGHT SECURITY ROUTE";
    [SerializeField] private string routeCompleteMessage = "Patrol route complete.";
    [SerializeField] private PatrolBoardRow[] rows;

    private static PatrolBoardPanel openPanel;

    public static bool IsAnyPatrolBoardOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        openPanel = null;
        IsAnyPatrolBoardOpen = false;
    }

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Show()
    {
        Refresh();

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        openPanel = this;
        IsAnyPatrolBoardOpen = true;
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

        IsAnyPatrolBoardOpen = openPanel != null;
    }

    public static void HideOpenPanel()
    {
        if (openPanel != null)
        {
            openPanel.Hide();
            return;
        }

        IsAnyPatrolBoardOpen = false;
    }

    private void Refresh()
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (PatrolManager.Instance == null)
        {
            SetCompleteText("No patrol route posted.");
            HideAllRows();
            return;
        }

        int taskCount = PatrolManager.Instance.TaskCount;

        for (int rowIndex = 0; rows != null && rowIndex < rows.Length; rowIndex++)
        {
            PatrolBoardRow row = rows[rowIndex];
            if (row == null)
            {
                continue;
            }

            int taskNumber = rowIndex + 1;
            if (taskNumber > taskCount)
            {
                row.Hide();
                continue;
            }

            bool isCompleted = taskNumber < PatrolManager.Instance.CurrentTaskNumber;
            bool isActive = taskNumber == PatrolManager.Instance.CurrentTaskNumber;
            row.SetTask(PatrolManager.Instance.GetTaskName(taskNumber), isCompleted, isActive);
        }

        SetCompleteText(PatrolManager.Instance.IsPatrolComplete ? routeCompleteMessage : "");
    }

    private void HideAllRows()
    {
        if (rows == null)
        {
            return;
        }

        foreach (PatrolBoardRow row in rows)
        {
            if (row != null)
            {
                row.Hide();
            }
        }
    }

    private void SetCompleteText(string message)
    {
        if (completeText == null)
        {
            return;
        }

        completeText.text = message;
        completeText.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
