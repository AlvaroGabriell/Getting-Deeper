using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;
    public AudioClip menuMusic;
    [SerializeField] private Slider musicSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            // DontDestroyOnLoad(gameObject); // Mantém o objeto entre as cenas
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (menuMusic != null)
        {
            PlayMenuMusic(false, menuMusic);
        }

        musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
    }

    public static void SetVolume(float volume)
    {
        Instance.audioSource.volume = volume;
    }

    public void PlayMenuMusic(bool resetSong, AudioClip newSong = null)
    {
        if (newSong != null)
        {
            audioSource.clip = newSong;
        }
        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                audioSource.Stop(); // Para a música atual se resetSong for verdadeiro
            }
            audioSource.Play();
        }
    }

    public void pauseMenuMusic()
    {
        audioSource.Pause(); // Pausa a música
    }
    
    public IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Pause();
        audioSource.volume = startVolume; // volta volume ao original para a próxima vez
    }
}
