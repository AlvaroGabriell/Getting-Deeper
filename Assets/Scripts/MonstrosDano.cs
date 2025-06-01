using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstrosDano : MonoBehaviour
{
    public float dano; //dano inflingido pelos monstros à Beth
                       //diferente para cada monstro?
    public PlayerVida playerVida;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerVida.LevarDano(dano);
        }
    }
}
