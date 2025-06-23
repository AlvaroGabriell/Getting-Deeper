using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monstro_Estado_JogadorDetectado : Monstro_Estado_Base
{
    public Monstro_Estado_JogadorDetectado(MonstroController monstro, string nomeAnimacao) : base(monstro, nomeAnimacao)
    {

    }

    public override void Enter()
    {
        base.Enter();
        //mudança da animação para wake
    }

    public override void Exit()
    {
        base.Exit();
        //retorno da animação pra idle
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!monstro.DetectarJogador()) //se o jogador não está mais "em range"
        {
            monstro.MudarEstado(monstro.estado_Idle);
        }
        else
        {
            //se o jogador está em alcance
            if (Time.time >= monstro.tempoEstado + monstro.delayAtaque)
            {// e deu o tempo do delay de o monstro ter registrado o jogador
                monstro.MudarEstado(monstro.estado_Agressivo);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

