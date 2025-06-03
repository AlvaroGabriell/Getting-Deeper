using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXLibrary : MonoBehaviour
{
    // Singleton instance
    public static SFXLibrary Instance { get; private set; }
    [SerializeField] private SFXGroup[] sfxGroups;
    // Dicionário para armazenar os grupos de SFX de forma eficiente
    private Dictionary<string, List<AudioClip>> sfxDictionary;
    private void Awake()
    {
        // Inicializa o singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o objeto entre as cenas
        }
        else
        {
            Destroy(gameObject); // Garante que apenas uma instância exista
        }

        // Inicializa o dicionário de SFX
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        sfxDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SFXGroup sfxGroup in sfxGroups)
        {
            sfxDictionary[sfxGroup.name] = sfxGroup.audioClips;
        }
    }

    // Método para obter um SFX aleatório de um grupo específico
    public AudioClip GetRandomSFX(string name)
    {
        if (sfxDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = sfxDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }
        return null;
    }
}

// Estrutura para armazenar grupos de SFX
// Os grupos são definidos no Unity Editor e servem pra randomizar SFX, pra não soar repetitivo
[System.Serializable]
public struct SFXGroup
{
    public string name;
    public List<AudioClip> audioClips;
}