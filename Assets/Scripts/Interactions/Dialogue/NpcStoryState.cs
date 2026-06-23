using System;
using UnityEngine;

[Serializable]
public class NpcStoryState
{
    [SerializeField] private int minimumStoryBeat;
    [SerializeField] private Transform location;
    [SerializeField] private bool isVisible = true;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private DialogueLine[] lines;
    [SerializeField] private int storyBeatAfterDialogue = -1;

    public int MinimumStoryBeat => minimumStoryBeat;
    public bool IsVisible => isVisible;
    public bool IsInteractable => isInteractable;
    public DialogueLine[] Lines => lines;
    public bool HasDialogue => lines != null && lines.Length > 0;
    public int StoryBeatAfterDialogue => storyBeatAfterDialogue;
    public bool AdvancesStoryAfterDialogue => storyBeatAfterDialogue >= 0;

    public void ApplyLocation(Transform npcTransform)
    {
        if (location == null || npcTransform == null)
        {
            return;
        }

        npcTransform.SetPositionAndRotation(location.position, location.rotation);
    }
}
