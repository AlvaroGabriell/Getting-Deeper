using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monstro_Estado_Idle : Monstro_Estado_Base
{
    public Monstro_Estado_Idle(MonstroController monstro, string nomeAnimacao) : base(monstro, nomeAnimacao)
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
        //inserir detecção de jogador
        if (monstro.DetectarJogador())
        {
            monstro.MudarEstado(monstro.estado_JogadorDetectado);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
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
