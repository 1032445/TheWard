using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class StoryBeatDebugHotkeys : MonoBehaviour
{
    [SerializeField] private bool hotkeysEnabled = true;
    [SerializeField] private int highestStoryBeat = 5;
    [SerializeField] private bool logChanges = true;

    private void Update()
    {
#if !UNITY_EDITOR
        return;
#endif

        if (!hotkeysEnabled || Keyboard.current == null || StoryProgress.Instance == null)
        {
            return;
        }

        int maxBeat = Mathf.Clamp(highestStoryBeat, 0, 5);

        for (int storyBeat = 0; storyBeat <= maxBeat; storyBeat++)
        {
            if (WasPressedThisFrame(Keyboard.current, storyBeat))
            {
                SetStoryBeat(storyBeat);
                return;
            }
        }
    }

    private bool WasPressedThisFrame(Keyboard keyboard, int number)
    {
        KeyControl numberRowKey = GetNumberRowKey(keyboard, number);

        return numberRowKey != null && numberRowKey.wasPressedThisFrame;
    }

    private KeyControl GetNumberRowKey(Keyboard keyboard, int number)
    {
        switch (number)
        {
            case 0: return keyboard.digit0Key;
            case 1: return keyboard.digit1Key;
            case 2: return keyboard.digit2Key;
            case 3: return keyboard.digit3Key;
            case 4: return keyboard.digit4Key;
            case 5: return keyboard.digit5Key;
            default: return null;
        }
    }

    private void SetStoryBeat(int storyBeat)
    {
        StoryProgress.Instance.SetStoryBeat(storyBeat);

        if (logChanges)
        {
            Debug.Log($"Debug story beat set to {storyBeat}.");
        }
    }
}
