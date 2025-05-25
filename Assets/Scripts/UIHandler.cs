using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject MenuInicial;
    public GameObject MenuSettings;
    public GameObject GameOverMenu;
    public GameObject player;
    bool gameStarted = false;
    bool playerReachedPosition = false;

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
            playerReachedPosition = playerMovement.walkOnScreen(); // Move o jogador até a posição de entrada
        }
    }

    // Função chamada quando o jogador clica no botão "Play"
    public void OnPlay()
    {
        MenuInicial.SetActive(false); // desativa o menu inicial
        gameStarted = true; // ativa a animação de entrada
    }

    // Função chamada quando o jogador clica no botão "Settings"
    public void OnSettings()
    {
        MenuSettings.SetActive(true); // ativa o menu de configurações
        MenuInicial.SetActive(false); // desativa o menu inicial
    }

    public void OnQuit()
    { //Função a ser chamada ao clicar o botão "Quit Game"
        Debug.Log("Fechando"); //teste para editor
        Application.Quit();
    }

    public void OnBack()
    {
        MenuSettings.SetActive(false); // desativa o menu de configurações
        MenuInicial.SetActive(true); // ativa o menu inicial
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
