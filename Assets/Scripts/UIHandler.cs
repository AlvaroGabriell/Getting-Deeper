using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public PlayerController playerController;
    public GameController gameController;
    public GameObject MenuInicial, MenuSettings, PauseMenu, AreYouSure, GameOverMenu, HintNoteMenu, BlackScreen;
    public GameObject player;
    public Sprite[] dicasSprites = new Sprite[3]; // Array para armazenar as sprites das dicas
    private CanvasGroup canvasGroup;
    bool gameStarted = false, playerReachedPosition = false, fadeIn = false, fadeOut = false;
    public static bool retryGameFromStart = false;
    private Stack<GameObject> menuStack = new Stack<GameObject>();

    // Use essa função sempre que for necessário abrir um menu
    // Ela garante que apenas um menu esteja ativo por vez, fechando o menu atual antes de abrir o novo
    // Mantendo uma pilha de menus para gerenciar a navegação entre eles.
    public void AbrirMenu(GameObject menu)
    {
        if (menuStack.Count > 0) // Se o há um menu ativo na pilha
        {
            menuStack.Peek().SetActive(false); // Desativa o menu atual no topo da pilha
        }

        menu.SetActive(true); // Ativa o novo menu
        menuStack.Push(menu); // Adiciona o novo menu no topo da pilha
    }

    // Use essa função para fechar o menu atual e reativar o anterior, se houver
    public void FecharMenuAtual()
    {
        if (menuStack.Count > 0)
        {
            menuStack.Pop().SetActive(false); // Fecha o menu atual e remove da pilha
        }
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetActive(true); // Reativa o menu anterior
        }
    }

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
        if (retryGameFromStart)
        {
            retryGameFromStart = false;
            OnPlay(); // Inicia o jogo novamente se a variável de retry estiver ativa
        }
        else
        {
            player.GetComponent<PlayerInput>().enabled = false; // desativa o controle do jogador
            AbrirMenu(MenuInicial); // Abre o menu inicial
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Responsável pela cinematic de entrada do jogo
        if (gameStarted && !playerReachedPosition)
        {
            playerReachedPosition = playerController.walkOnScreen(); // Move o jogador até a posição de entrada
        }

        // Responsável por aplicar o fade in e fade out nos menus
        if (fadeIn)
        {
            if (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime; // Aumenta a opacidade do menu com o tempo
            }
            else
            {
                fadeIn = false; // Para o fade in quando a opacidade atingir 1
            }
        }
        if (fadeOut)
        {
            if (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime; // Diminui a opacidade do menu com o tempo
            }
            else
            {
                fadeOut = false; // Para o fade out quando a opacidade atingir 0
                FecharMenuAtual(); // Fecha o menu após o fade out
            }
        }
    }

    // Main Menu
    // Função chamada quando o jogador clica no botão "Play"
    public void OnPlay()
    {
        MusicManager.Instance.startSwitchMusicWithFade(1.5F, 1.5F, MusicManager.Instance.gameplayMusic); // Inicia a música de gameplay
        FadeOutMenu(MenuInicial); // Inicia o fade out do menu inicial
        gameStarted = true; // ativa a animação de entrada
    }

    // Função chamada quando o jogador clica no botão "Settings"
    public void OnSettings()
    {
        AbrirMenu(MenuSettings);
        foreach (Slider slider in MenuSettings.GetComponentsInChildren<Slider>())
        {
            if (slider.name == "MusicSlider") MusicManager.Instance.AttachSlider(slider); // Anexa o slider de volume do menu de configurações
            else if (slider.name == "SFXSlider") SFXManager.Instance.AttachSlider(slider); // Anexa o slider de volume do SFX no menu de configurações
        }
    }

    public void OnQuit()
    { //Função a ser chamada ao clicar o botão "Quit Game"
        Debug.Log("Fechando"); //teste para editor
        Application.Quit();
    }

    // Settings Menu
    public void OnBack()
    {
        FecharMenuAtual(); // Fecha o menu atual e reativa o anterior
    }

    public void OnResume()
    {
        FecharMenuAtual(); // Fecha o menu de pausa
        gameController.ResumeGame(); // retoma o jogo
        player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable(); // Reativa o mapa de ações do jogador
    }

    public void OnAreYouSure()
    {
        AbrirMenu(AreYouSure); // Abre o menu de confirmação
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f; // Retoma o tempo do jogo
        SceneManager.LoadScene("Principal"); // Retorna ao menu principal
        MusicManager.Instance.PlayMusic(true, MusicManager.Instance.menuMusic); // Reinicia a música do menu
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
        else if (HintNoteMenu.activeSelf)
        {
            // Se estiver no menu de dica, fecha o menu de dica
            FecharDica();
        }
        else
        {
            // Se estiver no jogo rodando, pausa o jogo
            gameController.PauseGame();
            AbrirMenu(PauseMenu); // Abre o menu de pausa
            player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable(); // Desativa TODO o mapa de ações do jogador
            player.GetComponent<PlayerInput>().actions.FindAction("Pausar").Enable(); // Reativa a ação de pausar para permitir que o jogador retome o jogo
        }
    }

    public void FadeInMenu(GameObject menu)
    { // Se for usar a função, verifica se ela tá funcionando corretamente por causa do "AbrirMenu"
        canvasGroup = menu.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f; // Reseta a opacidade do menu para 0
            AbrirMenu(menu); // Abre o menu
            fadeIn = true;
            canvasGroup.interactable = true; // Ativa a interatividade do menu durante o fade in
        }
        else
        {
            Debug.LogWarning("CanvasGroup component not found on the menu GameObject.");
        }
    }
    public void FadeOutMenu(GameObject menu)
    { // O "FecharMenuAtual" é chamado no Update, então não precisa chamar aqui
        canvasGroup = menu.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            fadeOut = true;
            canvasGroup.interactable = false; // Desativa a interatividade do menu durante o fade out
        }
        else
        {
            Debug.LogWarning("CanvasGroup component not found on the menu GameObject.");
        }
    }

    public void MostrarDica(int TipoDica)
    {
        player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable(); // Desativa o mapa de ações do jogador para evitar movimentação enquanto a dica é exibida
        player.GetComponent<PlayerInput>().actions.FindAction("Pausar").Enable(); // Reativa a ação de pausar para permitir que o jogador retome o jogo
        player.GetComponent<PlayerInput>().actions.FindActionMap("Player").FindAction("Interagir").Enable(); // Reativa a ação de interagir para permitir que o jogador feche o menu de dica pelo mesmo botão
        switch (TipoDica)
        {
            case 1:
                HintNoteMenu.transform.Find("Dica").GetComponent<Image>().sprite = dicasSprites[0]; // Echostase
                break;
            case 2:
                HintNoteMenu.transform.Find("Dica").GetComponent<Image>().sprite = dicasSprites[1]; // Aracnise
                break;
            case 3:
                HintNoteMenu.transform.Find("Dica").GetComponent<Image>().sprite = dicasSprites[2]; // Sarcófago Andorinha
                break;
            case 4:
                HintNoteMenu.transform.Find("Dica").GetComponent<Image>().sprite = dicasSprites[3]; // Mariposa
                break;
            case 5:
                HintNoteMenu.transform.Find("Dica").GetComponent<Image>().sprite = dicasSprites[4]; // Sombra
                break;
            default:
                Debug.LogWarning("Tipo de dica inválido: " + TipoDica); // Exibe um aviso se o tipo de dica for inválido
                return;
        }

        AbrirMenu(HintNoteMenu); // Abre o menu de dicas
    }
    public void FecharDica()
    {
        SFXManager.Instance.PlaySFX("LeaveNote");
        FecharMenuAtual(); // Fecha o menu de dicas
        player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable(); // Reativa o mapa de ações do jogador
    }

    public void chamarGameOver()
    {
        player.SetActive(false);
        AbrirMenu(GameOverMenu); // Abre o menu de Game Over
        player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
        Time.timeScale = 0f; // Pausa o jogo
    }

    public void RetryJogo()
    {
        retryGameFromStart = true;
        Time.timeScale = 1f; // Retoma o tempo do jogo
        SceneManager.LoadScene("Principal"); // Retorna ao menu principal
    }
}