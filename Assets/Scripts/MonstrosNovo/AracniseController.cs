using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AracniseController : MonoBehaviour
{
    enum State { Idle, Chasing, Returning }
    State currentState = State.Idle;

    [Header("Referências")]
    public Transform player;
    public Animator animator;

    [Header("Ajustes")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 0.1f;

    private Vector3 spawnPosition;
    private Rigidbody2D rb;
    private ChaseZoneTrigger chaseZoneTrigger;

    void Awake()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        chaseZoneTrigger = transform.parent.GetComponentInChildren<ChaseZoneTrigger>();
    }

    void Update()
    {
        switch (currentState)
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
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lanterna"))
        {
            if (chaseZoneTrigger.isPlayerInChaseZone)
            {
                currentState = State.Chasing;
            }
            // TODO: animator.Play("TransformingToAggressive");
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * moveSpeed;
    }

    void ReturnToSpawn()
    {
        Vector2 dir = spawnPosition - transform.position;
        if (dir.magnitude < stoppingDistance)
        {
            // Chegou no spawn
            rb.velocity = Vector2.zero;
            currentState = State.Idle;
            // TODO: animator.Play("TransformingToIdle");
        }
        else
        {
            rb.velocity = dir.normalized * moveSpeed;
        }
    }

    void CheckIfLanternIsHitting()
    {
        // Checa se a lanterna está atingindo a Aracnise
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Lanterna") && chaseZoneTrigger.isPlayerInChaseZone)
            {
                currentState = State.Chasing;
                // TODO: animator.Play("TransformingToAggressive");
                break;
            }
        }
    }

    public void OnPlayerEnteredChaseZone()
    {
        if (currentState == State.Returning)
        {
            // Se o jogador entrar na zona de perseguição enquanto o aracnise está retornando, ele volta a perseguir
            currentState = State.Chasing;
        }
    }

    public void OnPlayerExitedChaseZone()
    {
        currentState = State.Returning;
    }
}