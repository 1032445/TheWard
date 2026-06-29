using System.Text;
using UnityEngine;

public class PatrolBoard : MonoBehaviour
{
    [SerializeField] private string title = "NIGHT SECURITY ROUTE";
    [SerializeField] private string completeText = "Patrol route complete.";

    private static TextPanel textPanel;

    private void Start()
    {
        if (textPanel == null)
        {
            textPanel = FindAnyObjectByType<TextPanel>();
        }
    }

    public void Interact()
    {
        if (textPanel != null)
        {
            textPanel.Show(BuildBoardText());
        }
        else
        {
            Debug.LogWarning("no panel found in scene");
        }
    }

    private string BuildBoardText()
    {
        if (PatrolManager.Instance == null)
        {
            return "No patrol route posted.";
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine(title);
        builder.AppendLine();

        for (int taskNumber = 1; taskNumber <= PatrolManager.Instance.TaskCount; taskNumber++)
        {
            builder.Append(GetMarker(taskNumber));
            builder.Append(' ');
            builder.Append(taskNumber);
            builder.Append(". ");
            builder.AppendLine(PatrolManager.Instance.GetTaskName(taskNumber));
        }

        if (PatrolManager.Instance.IsPatrolComplete)
        {
            builder.AppendLine();
            builder.AppendLine(completeText);
        }

        return builder.ToString();
    }

    private string GetMarker(int taskNumber)
    {
        if (taskNumber < PatrolManager.Instance.CurrentTaskNumber)
        {
            return "[x]";
        }

        if (taskNumber == PatrolManager.Instance.CurrentTaskNumber)
        {
            return "[>]";
        }

        return "[-]";
    }
}
