using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource sfxSource;

    public AudioClip hitSound;
    public AudioClip killSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        sfxSource.volume = PlayerPrefs.GetFloat("MasterVolume", 10f) / 10f;
    }

    public void PlayHitSound()
    {
        if (IsMuted()) return;
        sfxSource.PlayOneShot(hitSound);
    }

    public void PlayKillSound()
    {
        if (IsMuted()) return;
        sfxSource.PlayOneShot(killSound);
    }

    public bool IsMuted()
    {
        return PlayerPrefs.GetInt("MuteAudio", 0) == 1;
    }
}