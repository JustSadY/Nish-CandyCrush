using UnityEngine;
using System.Collections.Generic;

public enum SoundType
{
    ClickEffect,
    Crushes
}

[System.Serializable]
public struct SoundData
{
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<SoundData> soundList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundType soundType)
    {
        SoundData data = soundList.Find(s => s.type == soundType);

        if (data.clip != null)
        {
            sfxSource.PlayOneShot(data.clip, data.volume);
        }
    }
}