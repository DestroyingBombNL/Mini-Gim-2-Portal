using System.Collections;
using System.Linq;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Playlist")]
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private float delayBetweenSongs = 5f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] soundEffects;

    [SerializeField] private AudioClip introSong;
    private int currentTrackIndex = 0;
    private Coroutine playlistCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MuteSoundEffects()
    {
        sfxSource.enabled = false;
    }

    public void UnmuteSoundEffects()
    {
        sfxSource.Stop();
        sfxSource.enabled = true;
        sfxSource.Stop();
    }

    public void PlayIntroSong()
    {
        musicSource.clip = introSong;
        musicSource.volume = 0.5f;
        musicSource.Play();
    }

    public void StartPlaylist()
    {
        musicSource.Stop();
        musicSource.volume = 1f;
        if (playlistCoroutine != null)
            StopCoroutine(playlistCoroutine);

        playlistCoroutine = StartCoroutine(PlayPlaylistLoop());
    }

    private IEnumerator PlayPlaylistLoop()
    {
        while (true)
        {
            // Get current track
            AudioClip track = playlist[currentTrackIndex];

            // Play music
            musicSource.clip = track;
            musicSource.Play();

            // Wait until finished
            yield return new WaitForSeconds(track.length);

            // Wait extra delay
            yield return new WaitForSeconds(delayBetweenSongs);

            // Move to next track
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Length;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float delay = 0f)
    {
        StartCoroutine(PlaySFXDelayed(clip, volume, delay));
    }

    private IEnumerator PlaySFXDelayed(AudioClip clip, float volume, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        sfxSource.PlayOneShot(clip, volume);
    }

    public AudioClip GetAudioClipBasedOnName(string name)
    {
        AudioClip clip = null;
        clip = soundEffects.FirstOrDefault(c => c != null && c.name == name);
        return clip;
    }


    public void StopMusic()
    {
        if (playlistCoroutine != null)
            StopCoroutine(playlistCoroutine);

        musicSource.Stop();
    }
}