using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string interactionName = "Object";

    public UnityEvent onInteract;

    public void TriggerInteract()
    {
        onInteract?.Invoke();
    }
}