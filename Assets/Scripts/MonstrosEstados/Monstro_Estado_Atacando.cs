using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monstro_Estado_Atacando : Monstro_Estado_Base
{
    public Monstro_Estado_Atacando(MonstroController monstro, string nomeAnimacao) : base(monstro, nomeAnimacao)
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

    }

    #region 
    //Ataque frames

    public override void AnimacaoFimAtaque() //Final do evento de monstro realizar um ataque
    {
        base.AnimacaoFimAtaque();
        //Debug.Log("AAAAAAAAAAAAAAAAAA");
        monstro.anim.SetBool("ataque", false); //após um ataque executado, mudar o estado do monstro
        monstro.MudarEstado(monstro.estado_Agressivo);
    }

    public override void AnimacaoAtacar() //Começo do evento de monstro realizar um ataque
    {
        base.AnimacaoAtacar();
        //Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBA");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(monstro.aggro_area.position, monstro.distancia_alcance_ataque, monstro.layersAtacavel);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageble damageble = hitCollider.GetComponent<IDamageble>(); //tudo o que pode levar dano está inserido e detectavel com o IDamageble
            if (damageble != null)
            {//se algo recebeu dano
                damageble.Dano(monstro.qtd_Dano);
            }
        }
        
    }
    #endregion
}
