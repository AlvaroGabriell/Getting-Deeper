using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player, groundCheck;
    public CinemachineVirtualCamera cam;
    SpriteRenderer sr;
    Rigidbody2D rb;

    public Animator animator;

    [Header("Player Movement")]
    public float moveSpeed = 3f, jumpForce = 5f;
    float horizontalMovement;
    bool keyShift = false, keyControl = false;

    [Header("Cinematic")]
    public Transform playerTargetPosition;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);


    private void OnEnable()
    {
        PlayerVida.PersonagemMorre += InterromperMovimento;
    }

    private void OnDisable()
    {
        PlayerVida.PersonagemMorre -= InterromperMovimento;
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = player.GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();

        AtivarMovimento();
    }

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {
        if (keyControl && !keyShift)
        {
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
        animator.SetFloat("velocidade_jogador", Mathf.Abs(horizontalMovement));
        IsGrounded();
    }

    //Chamado quando o jogador pressiona alguma tecla de movimento
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (horizontalMovement == -1)
        {
            sr.flipX = true;
        }
        else if (horizontalMovement == 1)
        {
            sr.flipX = false;
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
        animator.SetBool("estaPulando",true);
        if (IsGrounded())
        {
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetBool("estaPulando", false);
            }
            else if (context.canceled)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.5f);
            }
        }
    }

    private bool IsGrounded()
    {
        // Verifica se o jogador está tocando o chão
        if (Physics2D.OverlapBox(groundCheck.transform.position, groundCheckSize, 0f, groundLayer))
        {
            animator.SetBool("estaPulando",false);
            return true;
        }    
        return false;
    }

    //Chamado quando o jogador pressiona a tecla de pular
    //TODO: Implementar pulo

    //Chamado quando o jogador pressiona o botão de "Play" no menu inicial
    public bool walkOnScreen()
    {
        // Move o jogador até a posição desejada
        player.transform.position = Vector2.MoveTowards(
            player.transform.position,
            playerTargetPosition.position,
            1.5F * Time.deltaTime
        );

        // Se o jogador chegou à posição desejada, ativa a câmera e o controle do jogador
        if (Vector2.Distance(player.transform.position, playerTargetPosition.position) == 0)
        {
            cam.enabled = false;
            cam.Follow = player.transform;
            cam.transform.position.Set(-17.35f, -0.3472534f, -10f);
            cam.enabled = true;
            player.GetComponent<PlayerInput>().enabled = true;
            return true;
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.transform.position, groundCheckSize);
    }

    private void InterromperMovimento()
    {
        animator.enabled = false;
        rb.bodyType = RigidbodyType2D.Static; //interromper movimentação quando Beth morre e tela de game over é mostrada
    }

    private void AtivarMovimento()
    { //após gameover, reativar movimentos e animações
        animator.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic; //interromper movimentação quando Beth morre e tela de game over é mostrada
    }
}
