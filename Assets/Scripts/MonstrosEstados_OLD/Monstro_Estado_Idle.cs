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
        monstro.rb.velocity = Vector2.zero;
        //Monstro para de se mexer inicialmente ao detectar um jogador
    }
}
