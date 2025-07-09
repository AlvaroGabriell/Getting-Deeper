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
    public GameObject lanterna, luzCapacete;
    PolygonCollider2D playerCollider;
    SpriteRenderer sr;
    Rigidbody2D rb;
    GameObject nota;

    [Header("Player Control")]
    public float moveSpeed = 1.8f, jumpForce = 4.4f;
    float horizontalMovement = 0.0f, lastMoveDirection = 0.0f, tempoSegurandoPulo = 0.0f;
    bool keyShift = false, keyControl = false, keyX = false, isAiming = false;
    bool estaRastejando = false, estadoAnteriorRastejando = false, estaAgachado = false, estadoAnteriorAgachado = false, carregandoPulo = false;
    Vector2 aimDirection;

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
        player.SetActive(true);
        sr = player.GetComponent<SpriteRenderer>();
        rb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<PolygonCollider2D>();

        AtivarMovimento();
    }

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {
        // Pega os estados de agachamento e rastejamento
        PegarEstados();

        if (estaAgachado != estadoAnteriorAgachado) estadoAnteriorAgachado = estaAgachado;
        if (estaRastejando != estadoAnteriorRastejando) estadoAnteriorRastejando = estaRastejando;

        HandleMoviment();
        HandleLanternAiming();
    }

    /// Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        // Pega os estados de agachamento e rastejamento
        PegarEstados();

        // Ajusta a hitbox do jogador se o estado de agachamento mudou
        if (estaAgachado != estadoAnteriorAgachado)
        {
            AjustarHitboxAgachado(estaAgachado);
            estadoAnteriorAgachado = estaAgachado;
        }
        if (estaRastejando != estadoAnteriorRastejando)
        {
            AjustarHitboxRastejando(estaRastejando);
            estadoAnteriorRastejando = estaRastejando;
        }

        SetarVariaveisDoAnimator();
    }

    private void PegarEstados()
    { // Função para pegar os estados de agachamento, rastejamento e etc.
        estaRastejando = keyX && !keyShift;
        estaAgachado = keyControl && !keyShift && !estaRastejando;

        // Impede o jogador de levantar se não houver espaço suficiente acima
        if (!estaAgachado && estadoAnteriorAgachado && !CanGetUp())
        {
            // Força manter agachado mesmo que o jogador solte o CTRL
            estaAgachado = true;
        }
        if (!estaRastejando && estadoAnteriorRastejando && !CanGetUp())
        {
            // Força manter rastejando mesmo que o jogador aperte o X
            estaRastejando = true;
        }
    }

    private void SetarVariaveisDoAnimator()
    { // Função pra setar as variáveis do Animator
        if (carregandoPulo)
        {
            // A mecânica de (pulo x tempo segurando) é calculada na linha 181, mas o tempo é contado aqui
            tempoSegurandoPulo += Time.deltaTime * 2.2f;
            animator.SetFloat("tempoSegurandoPulo", tempoSegurandoPulo);
        }
        animator.SetBool("carregandoPulo", carregandoPulo);

        animator.SetFloat("velocidadeJogador", Mathf.Abs(horizontalMovement));
        animator.SetFloat("velocidadeY", rb.velocity.y);
        animator.SetBool("isGrounded", IsGrounded());

        if (estaRastejando)
        {
            animator.SetBool("correndo", false);
            animator.SetBool("rastejando", true);
            animator.SetBool("agachado", false);
        }
        else if (estaAgachado)
        {
            animator.SetBool("correndo", false);
            animator.SetBool("rastejando", false);
            animator.SetBool("agachado", true);
        }
        else if (keyShift && IsLookingWhereIsGoing())
        {
            animator.SetBool("correndo", true);
            animator.SetBool("agachado", false);
            animator.SetBool("rastejando", false);
        }
        else
        {
            animator.SetBool("correndo", false);
            animator.SetBool("agachado", false);
            animator.SetBool("rastejando", false);
        }
    }

    private void HandleMoviment()
    { // Função para lidar com o movimento do jogador. NÃO use essa função, ela já é chamada onde precisa.
        if (!carregandoPulo)
        { // Se o jogador não estiver carregando um pulo, processa o movimento
            float moveSpeedModifier = 1.0f;

            if (estaRastejando || (!estaRastejando && estadoAnteriorRastejando && !CanGetUp()))
            {
                // Se o jogador estiver agachado ou rastejando e não pode levantar, aplica uma velocidade reduzida
                moveSpeedModifier = 0.4f;
            }
            else if (estaAgachado || (!estaAgachado && estadoAnteriorAgachado && !CanGetUp()))
            {
                // Se o jogador estiver agachado ou rastejando e não pode levantar, aplica uma velocidade reduzida
                moveSpeedModifier = 0.5f;
            }
            else if (keyShift && !keyControl)
            {
                // Se o jogador estiver correndo, aplica uma velocidade maior
                moveSpeedModifier = 1.5f;
            }

            if (!IsLookingWhereIsGoing())
            {
                moveSpeedModifier = 0.5f; // Se o jogador não está olhando para onde está indo, reduz a velocidade
                if(estaAgachado || estaRastejando)
                {
                    moveSpeedModifier = 0.38f; // Se o jogador não está olhando para onde está indo e está agachado ou rastejando, reduz ainda mais a velocidade
                }
            }

            rb.velocity = new Vector2(horizontalMovement * moveSpeed * moveSpeedModifier, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            // Se o jogador estiver carregando um pulo, não processa movimento horizontal
        }
    }

    private void HandleLanternAiming()
    {
        if (isAiming)
        {
            Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
            aimDirection = ((Vector2)mouseWorldPosition - (Vector2)lanterna.transform.position).normalized;
        }
        else
        {
            aimDirection = new Vector2(lastMoveDirection, 0).normalized; // Usa a última direção de movimento se não estiver mirando
        }

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        lanterna.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Atualiza a direção do jogador com base na posição do mouse
        if (aimDirection.x < 0)
        {
            sr.flipX = true;

            if (estaAgachado) luzCapacete.transform.localPosition = new Vector3(-0.065F, 0.153f, luzCapacete.transform.localPosition.z);
            else if (estaRastejando) luzCapacete.transform.localPosition = new Vector3(-0.28F, -0.097f, luzCapacete.transform.localPosition.z);
            // Se o jogador não estiver agachado nem rastejando, mantém a posição da luz do capacete
            else luzCapacete.transform.localPosition = new Vector3(-0.065F, 0.25f, luzCapacete.transform.localPosition.z);

            if (estaAgachado) lanterna.transform.localPosition = new Vector3(-0.065f, 0.152f, lanterna.transform.localPosition.z);
            else if (estaRastejando) lanterna.transform.localPosition = new Vector3(-0.26f, -0.097f, lanterna.transform.localPosition.z);
            else lanterna.transform.localPosition = new Vector3(-0.065f, 0.25f, lanterna.transform.localPosition.z);

        }
        else
        {
            sr.flipX = false;

            if (estaAgachado) luzCapacete.transform.localPosition = new Vector3(0.065F, 0.153f, luzCapacete.transform.localPosition.z);
            else if (estaRastejando) luzCapacete.transform.localPosition = new Vector3(0.28F, -0.097f, luzCapacete.transform.localPosition.z);
            // Se o jogador não estiver agachado nem rastejando, mantém a posição da luz do capacete
            else luzCapacete.transform.localPosition = new Vector3(0.065F, 0.25f, luzCapacete.transform.localPosition.z);

            if (estaAgachado) lanterna.transform.localPosition = new Vector3(0.065f, 0.152f, lanterna.transform.localPosition.z);
            else if (estaRastejando) lanterna.transform.localPosition = new Vector3(0.26f, -0.097f, lanterna.transform.localPosition.z);
            else lanterna.transform.localPosition = new Vector3(0.065f, 0.25f, lanterna.transform.localPosition.z);
        }
    }

    public bool IsLookingWhereIsGoing()
    {
        // Verifica se o jogador está olhando para onde está indo
        return (horizontalMovement >= 0 && aimDirection.x >= 0) || (horizontalMovement <= 0 && aimDirection.x <= 0);
    }

    public bool IsAiming()
    {
        return isAiming;
    }
    public Vector2 GetAimDirection()
    {
        return aimDirection;
    }

    //Chamado quando o jogador pressiona alguma tecla de movimento
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (gameController.isPaused) return; // Não processa movimento se o jogo estiver pausado
        //if (carregandoPulo) return; // Não processa movimento se o jogador estiver carregando um pulo

        if (!isAiming)
        {
            if (horizontalMovement == -1)
            {
                sr.flipX = true;
                if (estaAgachado) luzCapacete.transform.localPosition = new Vector3(-0.065F, 0.153f, luzCapacete.transform.localPosition.z);
                else if (estaRastejando) luzCapacete.transform.localPosition = new Vector3(-0.28F, -0.097f, luzCapacete.transform.localPosition.z);
                // Se o jogador não estiver agachado nem rastejando, mantém a posição da luz do capacete
                else luzCapacete.transform.localPosition = new Vector3(-0.065F, 0.25f, luzCapacete.transform.localPosition.z);
            }
            else if (horizontalMovement == 1)
            {
                sr.flipX = false;
                if (estaAgachado) luzCapacete.transform.localPosition = new Vector3(0.065F, 0.153f, luzCapacete.transform.localPosition.z);
                else if (estaRastejando) luzCapacete.transform.localPosition = new Vector3(0.28F, -0.097f, luzCapacete.transform.localPosition.z);
                // Se o jogador não estiver agachado nem rastejando, mantém a posição da luz do capacete
                else luzCapacete.transform.localPosition = new Vector3(0.065F, 0.25f, luzCapacete.transform.localPosition.z);
            }
        }

        if (horizontalMovement != 0) lastMoveDirection = horizontalMovement; // Define a última direção de movimento do jogador
    }

    //Chamado quando o jogador pressiona a tecla de correr
    public void Correr(InputAction.CallbackContext context)
    {
        keyShift = context.control.IsPressed();
        if (!IsLookingWhereIsGoing()) keyShift = false; // Não permite correr se o jogador não está olhando para onde está indo
    }

    //Chamado quando o jogador pressiona a tecla de agachar
    public void Agachar(InputAction.CallbackContext context)
    {
        keyControl = context.control.IsPressed();
    }

    // Chamado quando o jogador pressiona a tecla de rastejar
    public void Rastejar(InputAction.CallbackContext context)
    {
        if (context.performed) keyX = !keyX; // Alterna o estado de rastejar ao pressionar a tecla X
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

    //Chamado quando o jogador pressiona Escape (ESC) ou o botão de pausa (Ainda não tem)
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
                luzCapacete.SetActive(false);
                SFXManager.Instance.PlaySFX("LanternOff");
            }
            else
            {
                lanterna.SetActive(true);
                luzCapacete.SetActive(true);
                SFXManager.Instance.PlaySFX("LanternOn");
            }
        }
    }

    public void Interagir(InputAction.CallbackContext context)
    {
        UIHandler uiHandler = FindObjectOfType<UIHandler>();
        if (context.performed)
        {
            if (uiHandler.HintNoteMenu.activeSelf)
            {
                uiHandler.FecharDica(); // Fecha o menu de dica se estiver aberto
                return;
            }
            if (nota != null)
            {
                SFXManager.Instance.PlaySFX("GetNote");
                FindObjectOfType<UIHandler>().MostrarDica(nota.GetComponent<NoteHandler>().GetNoteType());
            }
        }
    }

    public void Mirar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;
        }
        else if (context.canceled)
        {
            isAiming = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interativo"))
        {
            nota = collision.gameObject;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interativo"))
        {
            nota = null;
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

    public void AjustarHitboxAgachado(bool diminuir)
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
    public void AjustarHitboxRastejando(bool diminuir)
    {
        if (diminuir)
        {
            Vector2[] points = playerCollider.GetPath(0); // Pega o Path 0
            points[0] = new Vector2(-0.3390332f, -0.2603368f);
            points[1] = new Vector2(-0.3118572f, -0.4873199f);
            points[4] = new Vector2(0.3469619f, -0.4877162f);
            points[5] = new Vector2(0.347517f, -0.03548002f);
            points[6] = new Vector2(-0.3381195f, -0.03743219f);
            playerCollider.SetPath(0, points); // Define o Path 0 com os novos pontos
        }
        else
        {
            Vector2[] points = playerCollider.GetPath(0); // Pega o Path 0
            points[0] = new Vector2(-0.2803154f, 0.334312f);
            points[1] = new Vector2(-0.2803154f, 0.0462718f);
            points[4] = new Vector2(0.1242179f, -0.3435888f);
            points[5] = new Vector2(0.1575317f, 0.2855315f);
            points[6] = new Vector2(0.08915329f, 0.334312f);
            playerCollider.SetPath(0, points); // Define o Path 0 com os novos pontos
        }
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
    
    // ------------------------ Machinima ------------------------
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

        // Se o jogador chegou à posição desejada, ativa a câmera principal, desativa a câmera auxiliar e ativa o controle do jogador
        if (Vector2.Distance(player.transform.position, playerTargetPosition.position) < 0.01f)
        {
            animator.SetBool("cinematicaDePlay", false);
            mainCamera.GetComponent<Camera>().enabled = true;
            staticCamera.gameObject.SetActive(false);
            lanterna.SetActive(true);
            luzCapacete.SetActive(true);
            SFXManager.Instance.PlaySFX("LanternOn");
            player.GetComponent<PlayerInput>().enabled = true;
            return true;
        }

        return false;
    }
}