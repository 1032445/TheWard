using UnityEngine;

public class PatrolManager : MonoBehaviour
{
    public static PatrolManager Instance { get; private set; }

    [SerializeField] private string[] taskNames;
    [SerializeField] private int currentTaskNumber = 1;
    [SerializeField] private int storyBeatAfterTask = -1;
    [SerializeField] private int taskNumberThatChangesStory = -1;

    public int CurrentTaskNumber => currentTaskNumber;
    public int TaskCount => taskNames != null ? taskNames.Length : 0;
    public bool IsPatrolComplete => currentTaskNumber > TaskCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("multiple patrol managers found");
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public string GetTaskName(int taskNumber)
    {
        int index = taskNumber - 1;
        if (taskNames == null || index < 0 || index >= taskNames.Length)
        {
            return "";
        }

        return taskNames[index];
    }

    public PatrolTaskResult TryCompleteTask(int taskNumber)
    {
        if (taskNumber < currentTaskNumber)
        {
            return PatrolTaskResult.AlreadyDone;
        }

        if (taskNumber > currentTaskNumber)
        {
            return PatrolTaskResult.TooEarly;
        }

        currentTaskNumber++;

        if (taskNumber == taskNumberThatChangesStory)
        {
            AdvanceStory();
        }

        return PatrolTaskResult.Completed;
    }

    private void AdvanceStory()
    {
        if (storyBeatAfterTask < 0)
        {
            return;
        }

        if (StoryProgress.Instance == null)
        {
            Debug.LogWarning("no progress found in scene.");
            return;
        }

        StoryProgress.Instance.AdvanceToStoryBeat(storyBeatAfterTask);
    }
}
