using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoSeguirJogador : MonoBehaviour
{
    public Echostase_Behavior[] echosArray;
    public PlayerController playerAnim; //controlar animator da Beth
    public bool agachando = false, rastejando = false;

    void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player")){   
            agachando = playerAnim.animator.GetBool("agachado");
            rastejando = playerAnim.animator.GetBool("rastejando");
            foreach (Echostase_Behavior echo in echosArray)
            {
                if (agachando || rastejando)
                {
                    echo.agressivo = false;
                }
                else
                {
                    echo.agressivo = true;
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (Echostase_Behavior echo in echosArray)
            {
                    echo.agressivo = true;
                //Debug.Log("BBBB");
            }
            
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            foreach (Echostase_Behavior echo in echosArray)
            {
                echo.agressivo = false;
            }

        }
    }

}
