using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternaIluminar : MonoBehaviour
{
    //Ao encostar a luz da lanterna num monstro: aplicar o efeito segundo o game design

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Alvo")
        {
            //Destroy(collision.gameObject); //Removendo o inimigo ao tocar na luz da lanterna?
            //Aracnise - "Gameplay: Se o player apontar a lanterna diretamente para a aranha enquanto 
            // ela está disfarçada, ela se transforma e ataca o player.
                         

        }
    }
}
