using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monstro_Estado_Base //Todos os outros estados herdam desse
{ //Maquina de estados para os monstros
    protected MonstroController monstro;
    protected string nomeAnimacao;

    //Construtor
    public Monstro_Estado_Base(MonstroController monstro, string nomeAnimacao)
    {
        this.monstro = monstro;
        this.nomeAnimacao = nomeAnimacao;
    }

    public virtual void Enter()
    {
        //ao entrar num estado
        Debug.Log("Entrou no estado: "+nomeAnimacao);
        monstro.anim.SetBool(nomeAnimacao, true);
    }

    public virtual void Exit()
    {
        //ao sair de um estado
        Debug.Log("Saiu do estado: "+nomeAnimacao);
        monstro.anim.SetBool(nomeAnimacao, false);
    }

    public virtual void LogicUpdate()
    {
        //equivalente ao update em classes sem monobehaivor

    }

    public virtual void PhysicsUpdate()
    {
        //equivalente ao fixedupdate em classes sem monobehaivor
    }
    
     #region 
    //Ataque frames
    public virtual void AnimacaoFimAtaque() {
       
    }

    public virtual void AnimacaoAtacar() {
        
    }
    #endregion
}

