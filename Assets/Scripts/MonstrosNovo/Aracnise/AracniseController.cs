using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AracniseController : MonstroBase
{
    [Header("Referências")]
    public Transform player;
    public Animator animator;
    public Camera mainCamera;

    [Header("Ajustes")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 0.1f;

    private Vector3 spawnPosition;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private ChaseZoneTrigger chaseZoneTrigger;
    private bool isTransformando = false;
    private bool jaTransformou = false;

    void Awake()
    {
        sp = GetComponentInChildren<SpriteRenderer>();
        setCurrentState(State.Idle);
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        chaseZoneTrigger = transform.parent.GetComponentInChildren<ChaseZoneTrigger>();
    }

    void Update()
    {
        animator.SetFloat("velocityX", Math.Abs(rb.velocity.x));
        

        if (getCurrentState() == State.Returning && IsOutsideCamera(Camera.main))
        {
            setCurrentState(State.Idle);
            ResetAracnise();
        }

        switch (getCurrentState())
        {
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Returning:
                ReturnToSpawn();
                break;
            case State.Idle:
                CheckIfLanternIsHitting();
                break;
            default:
                CheckIfLanternIsHitting();
                break;
        }

        FlipSprite();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lanterna"))
        {
            if (chaseZoneTrigger.isPlayerInChaseZone)
            {
                if (jaTransformou) return; // Se já transformou, não faz nada
                setCurrentState(State.Chasing);
                animator.SetBool("Transformar", true);
                jaTransformou = true;
                isTransformando = true;
            }
        }
        if (collision.CompareTag("Player"))
        {
            setTocandoPlayer(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            setTocandoPlayer(false);
        }
    }

    void CheckIfLanternIsHitting()
    {
        if (jaTransformou) return;
        // Checa se a lanterna está atingindo a Aracnise
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Lanterna") && chaseZoneTrigger.isPlayerInChaseZone)
            {
                setCurrentState(State.Chasing);
                isTransformando = true;
                animator.SetBool("Transformar", true);
                jaTransformou = true;
                break;
            }
        }
    }

    void FlipSprite()
    {
        if (rb.velocity.x < -0.1f)
        {
            transform.Find("AracniseVisual").GetComponent<BoxCollider2D>().offset = new Vector2(0.0750062f, -0.0455805f);
            sp.flipX = false;
        }
        else if (rb.velocity.x > 0.1f)
        {
            transform.Find("AracniseVisual").GetComponent<BoxCollider2D>().offset = new Vector2(-0.07f, -0.0455805f);
            sp.flipX = true;
        }
    }

    void ChasePlayer()
    {
        if (isTransformando) return;
        if (!isTocandoPlayer())
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * moveSpeed, 0f);
        }
        if (isTocandoPlayer())
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Atacar");
        }
    }

    void ReturnToSpawn()
    {
        Vector2 dir = spawnPosition - transform.position;
        if (dir.magnitude < stoppingDistance)
        {
            // Chegou no spawn
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = dir.normalized * moveSpeed;
        }
    }

    void ResetAracnise()
    {
        transform.position = spawnPosition;
        sp.flipX = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("Transformar", false);
        setCurrentState(State.Idle);
        animator.Play("Aracnise_Idle");
        isTransformando = false;
        jaTransformou = false;
    }

    bool IsOutsideCamera(Camera cam)
    {
        Bounds bounds = transform.Find("AracniseVisual").GetComponent<BoxCollider2D>().bounds;

        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y); // inferior esquerdo
        corners[1] = new Vector3(bounds.min.x, bounds.max.y); // superior esquerdo
        corners[2] = new Vector3(bounds.max.x, bounds.min.y); // inferior direito
        corners[3] = new Vector3(bounds.max.x, bounds.max.y); // superior direito

        foreach (var corner in corners)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(corner);

            if (viewPos.z > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return false;
            }
        }

        return true; // Todos os cantos estão fora da tela
    }

    public void OnPlayerEnteredChaseZone()
    {
        if (getCurrentState() == State.Returning)
        {
            // Se o jogador entrar na zona de perseguição enquanto o aracnise está retornando, ele volta a perseguir
            setCurrentState(State.Chasing);
        }
    }

    public void OnPlayerExitedChaseZone()
    {
        setCurrentState(State.Returning);
    }
    
    public void FinalizarTransformacao()
    {
        if (isTransformando)
        {
            isTransformando = false;
            animator.SetBool("Transformar", false);
        }
    }
}