using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstroController : MonoBehaviour
{ //Com base no Enemy do @NightRunStudio: https://www.youtube.com/watch?v=mWnboBD2vB8

    #region Variaveis

    //Estados que os inimigos podem assumir
    public Monstro_Estado_Base estado_Atual;
    public Monstro_Estado_JogadorDetectado estado_JogadorDetectado;
    public Monstro_Estado_Idle estado_Idle;
    public Monstro_Estado_Agressivo estado_Agressivo;
    public Monstro_Estado_Atacando estado_Atacando;

    public Animator anim;

    //Variáveis e controles de informações pertinentes a cada monstro
    public Rigidbody2D rb;
    public Transform aggro_area;
    public LayerMask playerLayer, layersAtacavel; //22/06/25 - ver se precisa detectar cenário  
    public int viradoP_Esquerda = 1;
    public float distancia_detectar_Jogador, distancia_alcance_ataque; //22/06/25 - ver se precisa detectar cenário  
    public float velocidade; //Velocidade que o monstro se move
    public float detectDelay; //Intervalo de tempo após detectar jogador para realizar operações
    public float tempoEstado; //Tempo passado após uma mudança de estados
    public float delayAtaque = 1; //Após Detectar inimigo o quanto de tempo demora para atacar
    public float tempo_Ataque, velocidade_Ataque; //duração e velocidade dos ataques
    public float qtd_Dano; //Quanto de dano aquele monstro dá ao atacar a Beth

    #endregion

    private void Awake()
    {
        estado_Idle = new Monstro_Estado_Idle(this, "idle");
        estado_JogadorDetectado = new Monstro_Estado_JogadorDetectado(this, "jogadorDetectado");
        estado_Agressivo = new Monstro_Estado_Agressivo(this, "agressivo");
        estado_Atacando = new Monstro_Estado_Atacando(this, "ataque");

        estado_Atual = estado_Idle;
        estado_Atual.Enter();
    }
    // Update is called once per frame
    void Update()
    {
        estado_Atual.LogicUpdate();
    }

    void FixedUpdate()
    {
        estado_Atual.PhysicsUpdate();

        
    }

    public bool DetectarJogador()
    {
        RaycastHit2D hitPlayer = Physics2D.Raycast(aggro_area.position, viradoP_Esquerda == 1 ? Vector2.left : Vector2.right, distancia_detectar_Jogador, playerLayer);
        //emptyobject aggro_area irá ver se o jogador está por perto
        if (hitPlayer.collider == true)
        {
            //Defindo Lógica para quando o jogador for detectado
            //StartCoroutine(JogadorDetectado());
            return true;
        }
        else //Jogador saiu do alcance do monstro depois de ser detectado uma vez
        {
            //StartCoroutine(JogadorNaoDetectado());
            return false;
        }
    }

    public bool DetectarAtacavel()
    {
        RaycastHit2D hitTarget = Physics2D.Raycast(aggro_area.position, viradoP_Esquerda == 1 ? Vector2.left : Vector2.right, distancia_alcance_ataque, playerLayer);
        if (hitTarget.collider == true)
        {
            return true;
        }
        else //Jogador saiu do alcance do monstro depois de ser detectado uma vez
        {
            return false;
        }

    }

    private void OnDrawGizmos() //Visualização do alcance de "visão" dos monstros
    {
        Gizmos.DrawRay(aggro_area.position, (viradoP_Esquerda == 1 ? Vector2.left : Vector2.right) * distancia_detectar_Jogador);
    }

    public void InverterSprite()
    {
        transform.Rotate(0, 180, 0);
        viradoP_Esquerda = -viradoP_Esquerda;
    }
    //mudança de estados da... máquina de estados
    public void MudarEstado(Monstro_Estado_Base novoEstado)
    {
        estado_Atual.Exit();
        estado_Atual = novoEstado;
        estado_Atual.Enter();
        tempoEstado = Time.time;
    }

    #region 
    //Ataque frames

    public void AnimacaoFimAtaque()
    {
        estado_Atual.AnimacaoFimAtaque();
    }

    public void AnimacaoAtacar()
    {
        estado_Atual.AnimacaoAtacar();
    }
    #endregion
}
