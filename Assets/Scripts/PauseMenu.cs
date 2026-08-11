using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private PlayerController playerController;
    private PlayerInteractor playerInteractor;
    private bool isPaused;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        playerInteractor = FindAnyObjectByType<PlayerInteractor>();
        ShowPausePanel(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused && controlsPanel != null && controlsPanel.activeSelf)
            {
                BackToPauseMenu();
            }
            else
            {
                SetPaused(!isPaused);
            }
        }
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void OpenControls()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    public void BackToPauseMenu()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        if (paused)
        {
            BackToPauseMenu();
        }
        else
        {
            ShowPausePanel(false);
        }

        if (playerController != null)
        {
            playerController.enabled = !paused;
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = !paused;
        }
    }

    private void ShowPausePanel(bool visible)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(visible);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
