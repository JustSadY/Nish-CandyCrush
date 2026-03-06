using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { private set; get; }

    [SerializeField] private GameObject PauseGameObject;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    private bool _bGameIsPaused = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        settingsButton.onClick.AddListener(ToggleSettings);
        musicButton.onClick.AddListener(ToggleMusicMute);
        homeButton.onClick.AddListener(ToggleHome);
        restartButton.onClick.AddListener(ToggleRestart);
    }

    private void Start()
    {
        PauseGameObject.SetActive(false);
        musicButton.GetComponent<Image>().sprite =
            Settings.IsMusicMuted() ? Settings.GetMutedMusicIcon() : Settings.GetUnMutedMusicIcon();
    }

    private void ToggleMusicMute()
    {
        Settings.SetMusicMuted(!Settings.IsMusicMuted());
        musicButton.GetComponent<Image>().sprite =
            Settings.IsMusicMuted() ? Settings.GetMutedMusicIcon() : Settings.GetUnMutedMusicIcon();

        SoundManager.Instance.StopSound();

        if (!Settings.IsMusicMuted())
        {
            SoundManager.Instance.PlaySound();
        }
    }

    private void ToggleHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ToggleRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ToggleSettings()
    {
        PauseGameObject.SetActive(!PauseGameObject.activeSelf);
        _bGameIsPaused = PauseGameObject.activeSelf;
    }

    public bool IsGamePaused() => _bGameIsPaused;
}