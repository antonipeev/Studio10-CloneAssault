using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;
    public AudioSource musicSource;  // Drag the AudioSource here in the Inspector

    void Awake()
    {
        // Ensure only one instance of the music player exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps music playing between scenes
        }
        else
        {
            Destroy(gameObject); // Prevents duplicate music players
        }
    }

    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.loop = true;  // Make sure it loops
            musicSource.Play();
        }
    }

    // Function to control music playback
    public void ToggleMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.Play();
        }
    }
}
