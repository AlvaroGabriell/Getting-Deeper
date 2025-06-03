using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monstro_Estado_Base
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

    }

    public virtual void LogicUpdate()
    {
        //equivalente ao update em classes sem monobehaivor
    }

    public virtual void PhysicsUpdate()
    {
        //equivalente ao fixedupdate em classes sem monobehaivor
    }
}

