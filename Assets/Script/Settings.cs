using UnityEngine;

public static class Settings
{
    private static bool _bMusicMuted = false;
    private static Sprite _mutedMusicIcon;
    private static Sprite _unMutedMusicIcon;

    public static Sprite GetMutedMusicIcon()
    {
        if (_mutedMusicIcon == null)
        {
            _mutedMusicIcon = Resources.Load<Sprite>("Texture/Icons/Border/ic_music_off");
        }

        return _mutedMusicIcon;
    }

    public static Sprite GetUnMutedMusicIcon()
    {
        if (_unMutedMusicIcon == null)
        {
            _unMutedMusicIcon = Resources.Load<Sprite>("Texture/Icons/Border/ic_music_on");
        }

        return _unMutedMusicIcon;
    }

    public static bool IsMusicMuted() => _bMusicMuted;
    public static void SetMusicMuted(bool state) => _bMusicMuted = state;
}