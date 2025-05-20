using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIHandler : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject MenuInicial;
    public GameObject player;
    bool gameStarted = false;
    bool playerReachedPosition = false;

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

    public void OnQuit() { //Função a ser chamada ao clicar o botão "Quit Game"
        Debug.Log("Fechando"); //teste para editor
        Application.Quit();
    }
}
