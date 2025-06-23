using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerVida : MonoBehaviour, IDamageble
{
    public static event Action PersonagemMorre;
    public float vida, vidaMaxima;

    void Start()
    {
        vida = vidaMaxima; //TESTES: começar com 2 ou 1 vida
    }

    //dano controlado pela interface
    public void Dano(float qtd_Dano)
    {
        vida -= qtd_Dano;
    }

    private void Update()
    {
        if (vida <= 0)
        {
            vida = 0;
            Debug.Log("Você Morreu, GAME OVER.");
            //Chamando tela de gameover
            PersonagemMorre?.Invoke();
        }
    }
}
