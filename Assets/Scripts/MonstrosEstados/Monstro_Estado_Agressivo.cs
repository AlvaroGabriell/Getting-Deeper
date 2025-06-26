using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monstro_Estado_Agressivo : Monstro_Estado_Base
{
    //atacar por um tempo até o jogador sair do alcance ou monstro ser derrotado
    public Monstro_Estado_Agressivo(MonstroController monstro, string nomeAnimacao) : base(monstro, nomeAnimacao)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (Time.time >= monstro.tempoEstado + monstro.tempo_Ataque) //se o ataque acabou
        {
            //verificar se o jogador ainda está em alcance
            if (monstro.DetectarJogador())
            {
                Agressivo();
            }
            else
            {
                //senão...retornar idle
                monstro.MudarEstado(monstro.estado_Idle);
            }
        }
        else
        {
            if (monstro.DetectarAtacavel())
            {
                monstro.MudarEstado(monstro.estado_Atacando);
            }
            
        }
    }

    void Agressivo()
    {
        if (monstro.viradoP_Esquerda==1)
            {
                monstro.rb.velocity = new Vector2(-monstro.velocidade, monstro.rb.velocity.y);
            }
            else
            {
                monstro.rb.velocity = new Vector2(monstro.velocidade, monstro.rb.velocity.y);
            }
    }
}
  
