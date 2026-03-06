using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button musicButton;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button ExitGameButton;


    private void Awake()
    {
        musicButton.onClick.AddListener(ToggleMusicMute);
        StartGameButton.onClick.AddListener(ToggleStartGame);
        ExitGameButton.onClick.AddListener(ToggleExitGame);
    }

    private void Start()
    {
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

    private void ToggleExitGame()
    {
        Application.Quit();
    }

    private void ToggleStartGame()
    {
        SceneManager.LoadScene("GameScreen");
    }
}