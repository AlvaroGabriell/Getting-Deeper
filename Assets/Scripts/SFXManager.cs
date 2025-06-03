using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    private static AudioSource audioSource;
    private static SFXLibrary sfxLibrary;
    [SerializeField] public Slider sfxSlider;

    private void Awake()
    {
        // Inicializa o singleton
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            sfxLibrary = GetComponent<SFXLibrary>(); // Obtém a instância do SFXLibrary
            DontDestroyOnLoad(gameObject); // Mantém o objeto entre as cenas
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }
    }

    // Método para tocar um SFX pelo nome
    // O volume pode ser ajustado, mas o volume padrão é 1.0f
    public void PlaySFX(string sfxName, float volume = 1.0f)
    {
        AudioClip audioClip = sfxLibrary.GetRandomSFX(sfxName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { SetVolume(sfxSlider.value); });
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void AttachSlider(Slider slider)
    {
        if (slider != null)
        {
            sfxSlider = slider;
            // remove listeners antigos só por segurança
            sfxSlider.onValueChanged.RemoveAllListeners();
            // adiciona o listener para atualizar o volume
            sfxSlider.onValueChanged.AddListener(delegate { SetVolume(sfxSlider.value); });
            // atualize o valor visual do slider pro volume atual
            sfxSlider.value = audioSource.volume;
        }
    }
}
