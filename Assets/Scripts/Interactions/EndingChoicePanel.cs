using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class EndingChoicePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private TextMeshProUGUI cancelText;
    [SerializeField] private Key confirmKey = Key.E;
    [SerializeField] private Key alternateConfirmKey = Key.Enter;
    [SerializeField] private Key cancelKey = Key.Escape;
    [SerializeField] private Key alternateCancelKey = Key.Q;

    public static bool IsAnyEndingChoiceOpen { get; private set; }

    private static EndingChoicePanel openPanel;
    private Action onConfirm;
    private bool isBeingShown;
    private int shownFrame = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        openPanel = null;
        IsAnyEndingChoiceOpen = false;
    }

    private void Awake()
    {
        if (panelRoot != null && !isBeingShown)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOpen || Keyboard.current == null || Time.frameCount == shownFrame)
        {
            return;
        }

        if (WasPressed(confirmKey) || WasPressed(alternateConfirmKey))
        {
            Confirm();
        }
        else if (WasPressed(cancelKey) || WasPressed(alternateCancelKey))
        {
            Hide();
        }
    }

    public void Show(string title, string body, string confirmLabel, string cancelLabel, Action confirmAction)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (bodyText != null)
        {
            bodyText.text = body;
        }

        if (confirmText != null)
        {
            confirmText.text = $"[{confirmKey}] {confirmLabel}";
        }

        if (cancelText != null)
        {
            cancelText.text = $"[{cancelKey}] {cancelLabel}";
        }

        onConfirm = confirmAction;

        if (panelRoot != null)
        {
            isBeingShown = true;
            panelRoot.SetActive(true);
            isBeingShown = false;
        }

        openPanel = this;
        IsAnyEndingChoiceOpen = true;
        shownFrame = Time.frameCount;
    }

    public void Confirm()
    {
        Action confirmAction = onConfirm;
        Hide();
        confirmAction?.Invoke();
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (openPanel == this)
        {
            openPanel = null;
        }

        onConfirm = null;
        IsAnyEndingChoiceOpen = false;
    }

    public static void HideOpenPanel()
    {
        if (openPanel != null)
        {
            openPanel.Hide();
            return;
        }

        IsAnyEndingChoiceOpen = false;
    }

    private bool WasPressed(Key key)
    {
        KeyControl control = Keyboard.current[key];
        return control != null && control.wasPressedThisFrame;
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
