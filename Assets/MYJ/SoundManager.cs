using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public SoundCategory category;
    public bool loop = false;
    public float volume = 1f;
    public bool is3D = false;
}

public enum SoundCategory
{
    BGM,
    Mission,
    Customer,
    Monster,
    Player
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("���� ����Ʈ")]
    public List<Sound> sounds;

    private Dictionary<string, Sound> soundDict = new();
    private Dictionary<string, AudioSource> loopedSources = new();
    private List<AudioSource> sfxPool = new();
    private AudioSource bgmSource;

    [Header("Ǯ�� �ɼ�")]
    public int poolSize = 10;

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
            return;
        }

        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.name))
                soundDict[sound.name] = sound;
            else
                Debug.LogWarning($"�ߺ��� ���� �̸�: {sound.name}");
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        for (int i = 0; i < poolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sfxPool.Add(source);
        }
    }

    public void Play(string name)
    {
        if (!soundDict.TryGetValue(name, out var sound)) return;

        if (sound.category == SoundCategory.BGM)
        {
            PlayBGM(sound);
            return;
        }

        var src = GetAvailableAudioSource();
        Configure(src, sound);
        src.loop = false;
        src.Play();
    }

    public void PlayLoop(string name)
    {
        if (loopedSources.ContainsKey(name)) return;
        if (!soundDict.TryGetValue(name, out var sound)) return;

        var src = GetAvailableAudioSource();
        Configure(src, sound);
        src.loop = true;
        src.Play();
        loopedSources[name] = src;
    }

    public void Stop(string name)
    {
        if (loopedSources.TryGetValue(name, out var src))
        {
            src.Stop();
            loopedSources.Remove(name);
        }

        if (bgmSource.isPlaying && bgmSource.clip.name == name)
        {
            bgmSource.Stop();
        }
    }


    private void PlayBGM(Sound sound)
    {
        bgmSource.clip = sound.clip;
        bgmSource.volume = sound.volume;
        bgmSource.loop = sound.loop;
        bgmSource.spatialBlend = 0f;
        bgmSource.Play();
    }

    private void Configure(AudioSource src, Sound sound)
    {
        src.clip = sound.clip;
        src.volume = sound.volume;
        src.spatialBlend = sound.is3D ? 1f : 0f;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
                return src;
        }

        var newSrc = gameObject.AddComponent<AudioSource>();
        newSrc.playOnAwake = false;
        sfxPool.Add(newSrc);
        return newSrc;
    }
}
