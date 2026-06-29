using UnityEngine;

public class PatrolTaskObject : MonoBehaviour
{
    [SerializeField] private int taskNumber = 1;
    [TextArea(2, 5)]
    [SerializeField] private string completedMessage = "Checked.";
    [TextArea(2, 5)]
    [SerializeField] private string tooEarlyMessage = "This is scheduled later in the patrol.";
    [TextArea(2, 5)]
    [SerializeField] private string alreadyDoneMessage = "Already checked.";

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
        if (PatrolManager.Instance == null)
        {
            ShowMessage("No patrol route found.");
            return;
        }

        PatrolTaskResult result = PatrolManager.Instance.TryCompleteTask(taskNumber);

        if (result == PatrolTaskResult.Completed)
        {
            ShowMessage(completedMessage);
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

    private void ShowMessage(string message)
    {
        if (textPanel != null)
        {
            textPanel.Show(message);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }
}
