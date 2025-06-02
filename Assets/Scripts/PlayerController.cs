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
    BoxCollider2D playerCollider;
    SpriteRenderer sr;
    Rigidbody2D rb;

    [Header("Player Movement")]
    public float moveSpeed = 1.8f, jumpForce = 4.4f;
    float horizontalMovement = 0;
    bool keyShift = false, keyControl = false, estaAgachado = false, estadoAnteriorAgachado = false;

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
        playerCollider = player.GetComponent<BoxCollider2D>();

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
        if(gameController.isPaused) return; // Não processa movimento se o jogo estiver pausado

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
        if (IsGrounded())
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("pulo");
            }
            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.5f);
                animator.SetTrigger("pulo");
            }
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
            playerCollider.size = new Vector2(playerCollider.size.x, 0.7016722f); // Diminui o tamanho do collider ao agachar
            playerCollider.offset = new Vector2(playerCollider.offset.x, -0.1346932f); // Ajusta a posição do collider ao agachar
        }
        else
        {
            playerCollider.size = new Vector2(playerCollider.size.x, 0.8199721f); // Aumenta o tamanho do collider ao levantar
            playerCollider.offset = new Vector2(playerCollider.offset.x, -0.07554325f); // Ajusta a posição do collider ao levantar
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
}
