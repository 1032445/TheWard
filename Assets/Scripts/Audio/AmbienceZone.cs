using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AmbienceZone : MonoBehaviour
{
    [SerializeField] private AmbienceState state = AmbienceState.Default;
    [SerializeField] private int priority;

    public AmbienceState State => state;
    public int Priority => priority;

    private void Reset()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();
        zoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other) || AmbienceManager.Instance == null)
        {
            return;
        }

        AmbienceManager.Instance.RegisterZone(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other) || AmbienceManager.Instance == null)
        {
            return;
        }

        AmbienceManager.Instance.UnregisterZone(this);
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.GetComponentInParent<PlayerController>() != null;
    }
}
