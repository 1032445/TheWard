using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadableImagePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image contentImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private static ReadableImagePanel openPanel;

    public static bool IsAnyReadableImageOpen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        openPanel = null;
        IsAnyReadableImageOpen = false;
    }

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Show(Sprite image, string title, string description)
    {
        if (contentImage != null)
        {
            contentImage.sprite = image;
            contentImage.enabled = image != null;
        }

        if (titleText != null)
        {
            titleText.text = title;
            titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
            descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(description));
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        openPanel = this;
        IsAnyReadableImageOpen = true;
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (contentImage != null)
        {
            contentImage.sprite = null;
            contentImage.enabled = false;
        }

        if (openPanel == this)
        {
            openPanel = null;
        }

        IsAnyReadableImageOpen = openPanel != null;
    }

    public static void HideOpenPanel()
    {
        if (openPanel != null)
        {
            openPanel.Hide();
            return;
        }

        IsAnyReadableImageOpen = false;
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
