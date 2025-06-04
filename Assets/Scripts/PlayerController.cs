using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player;
    public GameObject groundCheck;
    public GameObject ceilingCheck;
    public Camera mainCamera, staticCamera;
    public GameController gameController;
    public Animator animator;
    public GameObject lanterna;
    PolygonCollider2D playerCollider;
    SpriteRenderer sr;
    Rigidbody2D rb;

    [Header("Player Movement")]
    public float moveSpeed = 1.8f, jumpForce = 4.4f;
    float horizontalMovement = 0.0f, tempoSegurandoPulo = 0.0f;
    bool keyShift = false, keyControl = false, estaAgachado = false, estadoAnteriorAgachado = false, carregandoPulo = false;

    [Header("Cinematic")]
    public Transform playerTargetPosition;

    [Header("Ground/Ceiling Check")]
    public LayerMask groundLayer;
    public Vector2 groundCheckSize = new Vector2(0.2f, 0.01f), ceilingCheckSize = new Vector2(0.2f, 0.01f);

    private void OnEnable()
    {
        PlayerVida.PersonagemMorre += DesativarMovimento;
    }

    private void OnDisable()
    {
        PlayerVida.PersonagemMorre -= DesativarMovimento;
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = player.GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<PolygonCollider2D>();

        AtivarMovimento();
    }

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {
        estaAgachado = keyControl && !keyShift;

        if (!estaAgachado && estadoAnteriorAgachado && !CanGetUp())
        {
            // Força manter agachado mesmo que o jogador solte o CTRL
            estaAgachado = true;
        }

        if (estaAgachado != estadoAnteriorAgachado) estadoAnteriorAgachado = estaAgachado;

        if (!carregandoPulo)
        { // Se o jogador não estiver carregando um pulo, processa o movimento
            if (estaAgachado || (!estaAgachado && estadoAnteriorAgachado && !CanGetUp()))
            {
                // Se não houver espaço suficiente acima, mantem a velocidade de agachamento
                rb.velocity = new Vector2(horizontalMovement * moveSpeed * 0.5f, rb.velocity.y);
            }
            else if (keyShift && !keyControl)
            {
                rb.velocity = new Vector2(horizontalMovement * moveSpeed * 1.5f, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            // Se o jogador estiver carregando um pulo, não processa movimento horizontal
        }
    }

    /// Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        estaAgachado = keyControl && !keyShift;

        // Impede o jogador de levantar se não houver espaço suficiente acima
        if (!estaAgachado && estadoAnteriorAgachado && !CanGetUp())
        {
            // Força manter agachado mesmo que o jogador solte o CTRL
            estaAgachado = true;
        }

        // Ajusta a hitbox do jogador se o estado de agachamento mudou
        if (estaAgachado != estadoAnteriorAgachado)
        {
            AjustarHitbox(estaAgachado);
            estadoAnteriorAgachado = estaAgachado;
        }

        if (carregandoPulo)
        {
            tempoSegurandoPulo += Time.deltaTime * 2.2f;
            animator.SetFloat("tempoSegurandoPulo", tempoSegurandoPulo);
        }
        animator.SetBool("carregandoPulo", carregandoPulo);

        animator.SetFloat("velocidadeJogador", Mathf.Abs(horizontalMovement));
        animator.SetFloat("velocidadeY", rb.velocity.y);
        animator.SetBool("isGrounded", IsGrounded());
        if (estaAgachado)
        {
            animator.SetBool("correndo", false);
            animator.SetBool("agachado", true);
        }
        else if (keyShift && !keyControl)
        {
            animator.SetBool("correndo", true);
            animator.SetBool("agachado", false);
        }
        else
        {
            animator.SetBool("correndo", false);
            animator.SetBool("agachado", false);
        }
    }

    //Chamado quando o jogador pressiona alguma tecla de movimento
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (gameController.isPaused) return; // Não processa movimento se o jogo estiver pausado
        if (carregandoPulo) return; // Não processa movimento se o jogador estiver carregando um pulo

        if (horizontalMovement == -1)
        {
            sr.flipX = true;
            lanterna.transform.rotation = Quaternion.Euler(0, 0, 90); // Inverte a lanterna quando o jogador se move para a esquerda
        }
        else if (horizontalMovement == 1)
        {
            sr.flipX = false;
            lanterna.transform.rotation = Quaternion.Euler(0, 0, -90); // Inverte a lanterna quando o jogador se move para a esquerda
        }
    }

    //Chamado quando o jogador pressiona a tecla de correr
    public void Correr(InputAction.CallbackContext context)
    {
        keyShift = context.control.IsPressed();
    }

    //Chamado quando o jogador pressiona a tecla de agachar
    public void Agachar(InputAction.CallbackContext context)
    {
        keyControl = context.control.IsPressed();
    }

    //Chamado quando o jogador pressiona a tecla de pular
    public void Pular(InputAction.CallbackContext context)
    {
        if (gameController.isPaused) return; // Não processa pulo se o jogo estiver pausado
        // Verifica se o jogador está no chão antes de pular
        if (context.started && IsGrounded())
        {
            tempoSegurandoPulo = 0.0f; // Reseta o tempo de pulo
            carregandoPulo = true;
            animator.SetTrigger("pulo");
        }

        if (context.canceled && carregandoPulo)
        {
            float forcaFinalPulo = Mathf.Clamp(tempoSegurandoPulo, 0.8f, 1.3f) * jumpForce; // Calcula a força do pulo com base no tempo segurando o botão
            rb.velocity = new Vector2(rb.velocity.x, forcaFinalPulo);
            carregandoPulo = false;
        }
    }

    //Chamado quando o jogador pressiona Escape (ESC) ou o botão de pausa
    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FindObjectOfType<UIHandler>().HandleEscape();
        }
    }

    public void MudarEstadoLanterna(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (lanterna.activeSelf)
            {
                lanterna.SetActive(false);
                SFXManager.Instance.PlaySFX("LanternOff");
            }
            else
            {
                lanterna.SetActive(true);
                SFXManager.Instance.PlaySFX("LanternOn");
            }
        }
    }

    private bool IsGrounded()
    { // Verifica se o jogador está tocando o chão
        return Physics2D.OverlapBox(groundCheck.transform.position, groundCheckSize, 0f, groundLayer);
    }

    private bool CanGetUp()
    { // Verifica se o jogador pode levantar
        return !Physics2D.OverlapBox(ceilingCheck.transform.position, ceilingCheckSize, 0f, groundLayer);
    }

    public void AjustarHitbox(bool diminuir)
    {
        if (diminuir)
        {
            Vector2[] points = playerCollider.GetPath(0); // Pega o Path 0
            points[0] = new Vector2(points[0].x, 0.2125836f); // Ajusta a posição do ponto 0
            points[5] = new Vector2(points[5].x, 0.2125836f); // Ajusta a posição do ponto 5
            points[6] = new Vector2(points[6].x, 0.2125836f); // Ajusta a posição do ponto 6
            playerCollider.SetPath(0, points); // Define o Path 0 com os novos pontos
        }
        else
        {
            Vector2[] points = playerCollider.GetPath(0); // Pega o Path 0
            points[0] = new Vector2(points[0].x, 0.334312f); // Ajusta a posição do ponto 0
            points[5] = new Vector2(points[5].x, 0.2855315f); // Ajusta a posição do ponto 5
            points[6] = new Vector2(points[6].x, 0.334312f); // Ajusta a posição do ponto 6
            playerCollider.SetPath(0, points); // Define o Path 0 com os novos pontos
        }
    }

    //Chamado quando o jogador pressiona o botão de "Play" no menu inicial
    public bool walkOnScreen()
    {
        // Move o jogador até a posição desejada
        animator.Play("Walk");
        animator.SetBool("cinematicaDePlay", true);
        player.transform.position = Vector2.MoveTowards(
            player.transform.position,
            playerTargetPosition.position,
            1.5F * Time.deltaTime
        );

        // Se o jogador chegou à posição desejada, ativa a câmera principal, desativa a auxiliar e ativa o controle do jogador
        if (Vector2.Distance(player.transform.position, playerTargetPosition.position) < 0.01f)
        {
            animator.SetBool("cinematicaDePlay", false);
            mainCamera.GetComponent<Camera>().enabled = true;
            staticCamera.gameObject.SetActive(false);
            player.GetComponent<PlayerInput>().enabled = true;
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.transform.position, groundCheckSize);
        Gizmos.DrawCube(ceilingCheck.transform.position, ceilingCheckSize);
    }

    public void DesativarMovimento()
    {
        animator.enabled = false;
        rb.bodyType = RigidbodyType2D.Static; // Interromper movimentação quando Beth morre e tela de game over é mostrada
    }

    public void AtivarMovimento()
    { // Ativa/Reativa o movimento do jogador
        animator.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic; // Reativar movimentação
    }
    
    // -------------- Player SFX Methods --------------
    public void PlaySteps()
    {
        SFXManager.Instance.PlaySFX("Steps");
    }
    public void PlayStepsCrouched()
    {
        SFXManager.Instance.PlaySFX("Steps", 0.5F);
    }
    public void PlayJump()
    {
        SFXManager.Instance.PlaySFX("Jump");
    }
    public void PlayLanding()
    {
        SFXManager.Instance.PlaySFX("Landing");
    }
}
