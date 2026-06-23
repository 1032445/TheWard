using System;
using UnityEngine;

public class StoryProgress : MonoBehaviour
{
    public static StoryProgress Instance { get; private set; }
    public static event Action<int> StoryBeatChanged;

    [SerializeField] private int currentStoryBeat;

    public int CurrentStoryBeat => currentStoryBeat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("multiple progress components found");
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

    public void SetStoryBeat(int storyBeat)
    {
        if (currentStoryBeat == storyBeat)
        {
            return;
        }

        currentStoryBeat = storyBeat;
        StoryBeatChanged?.Invoke(currentStoryBeat);
    }

    public void AdvanceToStoryBeat(int storyBeat)
    {
        if (storyBeat <= currentStoryBeat)
        {
            return;
        }

        SetStoryBeat(storyBeat);
    }
}
