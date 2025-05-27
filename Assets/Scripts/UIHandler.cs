using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public PlayerController playerController;
    public GameController gameController;
    public GameObject MenuInicial, MenuSettings, PauseMenu, GameOverMenu;
    public GameObject player;
    public MusicManager musicManager;
    bool gameStarted = false, playerReachedPosition = false;

    private void OnEnable()
    {
        PlayerVida.PersonagemMorre += chamarGameOver;
    }

    private void OnDisable()
    {
        PlayerVida.PersonagemMorre -= chamarGameOver;
    }
    // Start is called before the first frame update
    void Start()
    {
        player.GetComponent<PlayerInput>().enabled = false; // desativa o controle do jogador
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted && !playerReachedPosition)
        {
            playerReachedPosition = playerController.walkOnScreen(); // Move o jogador até a posição de entrada
        }
    }

        // Main Menu
    // Função chamada quando o jogador clica no botão "Play"
    public void OnPlay()
    {
        MenuInicial.SetActive(false); // desativa o menu inicial
        StartCoroutine(musicManager.FadeOutMusic(1.5f)); // pausa a música do menu com fade out
        gameStarted = true; // ativa a animação de entrada
    }

    // Função chamada quando o jogador clica no botão "Settings"
    public void OnSettings()
    {
        MenuSettings.SetActive(true); // ativa o menu de configurações
        MenuInicial.SetActive(false); // desativa o menu inicial, se estiver ativo
        PauseMenu.SetActive(false); // desativa o menu de pausa, se estiver ativo
    }

    public void OnQuit()
    { //Função a ser chamada ao clicar o botão "Quit Game"
        Debug.Log("Fechando"); //teste para editor
        Application.Quit();
    }

        // Settings Menu
    public void OnBack()
    {
        if (gameController.isPaused)
        {
            PauseMenu.SetActive(true); // ativa o menu de pausa
            MenuSettings.SetActive(false); // desativa o menu de configurações
        }
        else
        {
            MenuSettings.SetActive(false); // desativa o menu de configurações
            MenuInicial.SetActive(true); // ativa o menu inicial    
        }
    }

    public void OnResume()
    {
        gameController.ResumeGame(); // retoma o jogo
        playerController.AtivarMovimento(); // Ativa o movimento do jogador
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f; // Retoma o tempo do jogo
        SceneManager.LoadScene(0);
    }

    public void HandleEscape()
    {
        if (MenuSettings.activeSelf)
        {
            // Se estiver na tela de Settings, age como Back
            OnBack();
        }
        else if (PauseMenu.activeSelf)
        {
            // Se estiver no menu de pausa, resume o jogo
            OnResume();
        }
        else
        {
            // Se estiver no jogo rodando, pausa o jogo
            gameController.PauseGame();
            playerController.DesativarMovimento(); // Desativa o movimento do jogador
        }
    }

    public void chamarGameOver()
    {
        GameOverMenu.SetActive(true);
    }

    public void RetryJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
