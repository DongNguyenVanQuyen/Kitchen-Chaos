using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    private AudioSource audioSource;
    private float volume = 0.5f;
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();


        // Load volume từ PlayerPrefs
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.5f);
        audioSource.volume = volume;

    }
    public void ChangeVolume()
    {
        // Implementation for changing volume
        volume += 0.1f;
        volume = volume % 1.1f;
        if (volume < 0.1f)
        {
            volume = 0f;
        }
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();

    }
    public float GetVolume()
    {
        return volume;
    }
}
