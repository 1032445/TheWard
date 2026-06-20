using UnityEngine;

public class ReadableObject : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string content = "This is placeholder text.";

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
            textPanel.Show(content);
        }
        else
        {
            Debug.LogWarning("No TextPanel found in scene.");
        }
    }
}