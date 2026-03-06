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

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource _sfxSource;
    [SerializeField] private List<SoundData> soundList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _sfxSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySfx(SoundType soundType)
    {
        SoundData data = soundList.Find(s => s.type == soundType);

        if (data.clip != null)
        {
            _sfxSource.PlayOneShot(data.clip, data.volume);
        }
    }

    public void PlaySound()
    {
        _sfxSource.Play();
    }

    public void StopSound()
    {
        _sfxSource.Stop();
    }
}