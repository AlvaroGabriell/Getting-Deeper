using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dig2 : MonoBehaviour
{
    public Escorpiao_Behavior escorpiao; // Mudar estado agressivo do monstro
    /*Pegando informações referentes à
        Zona de Aggro 1 e 2;
    */

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {   
            escorpiao.scorpAnim.SetBool("agressivo", true);
            escorpiao.agressivo = true;
        }
    }


}
