using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escorpiao_Digging : MonoBehaviour
{
    public GameObject zona1, zona2;
    public Escorpiao_Behavior escorpiao; // Mudar estado agressivo do monstro
    /*Pegando informações referentes à
        Zona de Aggro 1 e 2;
    */

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { //Desligando a hitbox da zona1 e ligando a da zona 2 quando o escorpião ficar agressivo
            zona1.SetActive(false);
            zona2.SetActive(true);
            escorpiao.scorpAnim.SetBool("agressivo", true);
            escorpiao.agressivo = true;
        }
    }

    
}
