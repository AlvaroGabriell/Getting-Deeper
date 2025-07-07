using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escorpiao_Digging : MonoBehaviour
{
    public GameObject ativarZona, underground, aboveground, escorpiao_sprite;
    /*Pegando informações referentes à
        Zona de Aggro 2;
        Ponto que marca a altura Y do sprite abaixo do solo
        Ponto que marca a altura Y do sprite acima do solo
        Manipulando o próprio escorpião
    */

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ativarZona.SetActive(true);
        }
        //escopiao.transform.position.y
    }
}
