using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AracneSegueJogador : MonoBehaviour
{
 public Aracnise_Behavior[] aracArray;
    public PlayerController playerAnim; //controlar animator da Beth
    public bool luz = false;

    //Work in progress 05/07/25 - só fiz copiar os metodos do outro por hora

    void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player")){   
         
            foreach (Aracnise_Behavior arac in aracArray)
            {
                arac.agressivo = true;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (Aracnise_Behavior arac in aracArray)
            {
                    arac.agressivo = true;
                //Debug.Log("BBBB");
            }
            
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            foreach (Aracnise_Behavior arac in aracArray)
            {
                arac.agressivo = false;
            }

        }
    }
}
