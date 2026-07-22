using UnityEngine;

public class PatrolTaskObject : MonoBehaviour
{
    [SerializeField] private int taskNumber = 1;
    [SerializeField] private TextPanel textPanel;

    [Header("Completed Popup")]
    [SerializeField] private string completedTitle;
    [TextArea(2, 5)]
    [SerializeField] private string completedDescription;
    [TextArea(2, 5)]
    [SerializeField] private string completedMessage = "Checked.";
    [Header("Other Messages")]
    [TextArea(2, 5)]
    [SerializeField] private string tooEarlyMessage = "This is scheduled later in the patrol.";
    [TextArea(2, 5)]
    [SerializeField] private string alreadyDoneMessage = "Already checked.";

    public void Interact()
    {
        if (PatrolManager.Instance == null)
        {
            ShowMessage("No patrol route found.");
            return;
        }

        PatrolTaskResult result = PatrolManager.Instance.TryCompleteTask(taskNumber);

        if (result == PatrolTaskResult.Completed)
        {
            ShowCompletedMessage();
        }
        else if (result == PatrolTaskResult.TooEarly)
        {
            ShowMessage(tooEarlyMessage);
        }
        else
        {
            ShowMessage(alreadyDoneMessage);
        }
    }

    private void ShowCompletedMessage()
    {
        if (!string.IsNullOrEmpty(completedTitle) || !string.IsNullOrEmpty(completedDescription))
        {
            ShowMessage(completedTitle, completedDescription);
            return;
        }

        ShowSplitMessage(completedMessage);
    }

    private void ShowSplitMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            ShowMessage("");
            return;
        }

        string normalizedMessage = message.Replace("\r\n", "\n");
        int firstLineEnd = normalizedMessage.IndexOf('\n');

        if (firstLineEnd < 0)
        {
            ShowMessage(normalizedMessage);
            return;
        }

        string title = normalizedMessage.Substring(0, firstLineEnd);
        string description = normalizedMessage.Substring(firstLineEnd + 1).TrimStart('\n');
        ShowMessage(title, description);
    }

    private void ShowMessage(string title, string message)
    {
        if (textPanel != null)
        {
            textPanel.Show(title, message);
        }
        else
        {
            Debug.LogWarning("patrol task object has no text panel assigned.");
        }
    }

    private void ShowMessage(string message)
    {
        if (textPanel != null)
        {
            textPanel.Show(message);
        }
        else
        {
            Debug.LogWarning("patrol task object has no text panel assigned.");
        }
    }
}
