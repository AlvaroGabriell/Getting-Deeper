using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public GameObject pauseMenu, player;
    public bool isPaused = false;

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
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Pausa o jogo
        isPaused = true; // Atualiza o estado de pausa
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Retoma o jogo
        isPaused = false; // Atualiza o estado de pausa
    }
}
