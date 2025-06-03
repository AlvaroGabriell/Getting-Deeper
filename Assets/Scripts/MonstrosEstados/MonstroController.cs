using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstroController : MonoBehaviour
{ //Com base no Enemy do @NightRunStudio: https://www.youtube.com/watch?v=mWnboBD2vB8

    //Estados que os inimigos podem assumir
    public Monstro_Estado_Base estado_Base;

    public Monstro_Estado_JogadorDetectado estado_JogadorDetectado;
    public Monstro_Estado_Idle estado_Idle;

    //Variáveis e controles de informações pertinentes a cada monstro
    public Rigidbody2D rb;
    public LayerMask playerLayer;

    public bool viradoP_Esquerda = true;
    public float distancia_detectar_Jogador;
    public float velocidade;


    private void Awake()
    {
        estado_Idle = new Monstro_Estado_Idle(this, "idle");
        estado_JogadorDetectado = new Monstro_Estado_JogadorDetectado(this, "jogadorDetectado");

        estado_Base = estado_Idle;
        estado_Base.Enter();
    }
    // Update is called once per frame
    void Update()
    {
        estado_Base.LogicUpdate();
    }

    void FixedUpdate()
    {
        estado_Base.PhysicsUpdate();
    }

    void DetectarJogador()
    {
        
    }
}
