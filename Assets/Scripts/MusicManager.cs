using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    private static AudioSource audioSource;
    public AudioClip menuMusic;
    [SerializeField] public Slider musicSlider;
    private int mostRecentCoroutineID = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject); // Mantém o objeto entre as cenas
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
            startFadeIn(1.5F); // Inicia com um fade in de 1.5 segundos
        }

        musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
    }

    public void AttachSlider(Slider slider)
    {
        if (slider != null)
        {
            musicSlider = slider;
            // remove listeners antigos só por segurança
            musicSlider.onValueChanged.RemoveAllListeners();
            // adiciona o listener para atualizar o volume
            musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
            // atualize o valor visual do slider pro volume atual
            musicSlider.value = audioSource.volume;
        }
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
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

    public void startFadeOut(float duration)
    {
        mostRecentCoroutineID++;
        StartCoroutine(FadeOutMusic(duration, mostRecentCoroutineID));
    }
    public void startFadeIn(float duration)
    {
        mostRecentCoroutineID++;
        StartCoroutine(FadeInMusic(duration, mostRecentCoroutineID));
    }

    public IEnumerator FadeOutMusic(float duration, int myCoroutineID)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            if (myCoroutineID != mostRecentCoroutineID) yield break; // Verifica se a coroutine ainda é a mais recente, se não for, termina a execução
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Pause();
        audioSource.volume = startVolume; // volta volume ao original para a próxima vez
    }

    public IEnumerator FadeInMusic(float duration, int myCoroutineID)
    {
        float startVolume = 0f;
        float targetVolume = audioSource.volume;

        audioSource.volume = startVolume; // Começa com volume 0 para o fade in
        PlayMenuMusic(false, menuMusic); // Garante que a música esteja tocando

        while (audioSource.volume < targetVolume)
        {
            if (myCoroutineID != mostRecentCoroutineID) yield break; // Verifica se a coroutine ainda é a mais recente, se não for, termina a execução
            audioSource.volume += targetVolume * Time.deltaTime / duration;
            yield return null;
        }
    }
}
