using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject pauseMenu, player;
    public bool isPaused = false;

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
