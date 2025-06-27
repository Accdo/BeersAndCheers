using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float volume = 1f;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("사운드 리스트")]
    public List<Sound> sounds = new List<Sound>();

    private Dictionary<string, Sound> soundDict;
    private AudioSource audioSource;

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

        audioSource = gameObject.AddComponent<AudioSource>();
        soundDict = new Dictionary<string, Sound>();

        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.name))
                soundDict.Add(sound.name, sound);
        }
    }

    public void Play(string name)
    {
        if (soundDict.TryGetValue(name, out Sound sound))
        {
            audioSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"Sound '{name}' not found!");
        }
    }
}
