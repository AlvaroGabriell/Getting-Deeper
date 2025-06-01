using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerVida : MonoBehaviour
{
    public static event Action PersonagemMorre;
    public float vida, vidaMaxima;

    void Start()
    {
        vida = vidaMaxima; //TESTES: começar com 2 ou 1 vida
    }


    public void LevarDano(float dano) //dano é enviado pelo script de inimigo
    {
        vida -= dano;
        if (vida <= 0)
        {
            vida = 0;
            Debug.Log("Você Morreu, GAME OVER.");
            //Chamando tela de gameover
            PersonagemMorre?.Invoke();
        }
    }
}
