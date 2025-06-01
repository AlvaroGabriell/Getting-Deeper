using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstrosMovimento : MonoBehaviour
{
    public Transform[] pontosPatrulha;
    public float velocidade_movimento;
    public int destinoPatrulha;

    // Update is called once per frame
    void Update()
    {
        //Movimentação ponto a ponto (sendo os pontos os definidos previamente)
        if (destinoPatrulha == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, pontosPatrulha[0].position, velocidade_movimento * Time.deltaTime);
            if (Vector2.Distance(transform.position, pontosPatrulha[1].position) < .2f) {
                transform.localScale = new Vector3(1, 1, 1); //flipando o sprite
                destinoPatrulha = 1;
            }
        }
        
        if (destinoPatrulha == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, pontosPatrulha[0].position, velocidade_movimento * Time.deltaTime);
            if (Vector2.Distance(transform.position, pontosPatrulha[1].position) < .2f) {
                transform.localScale = new Vector3(-1,-1,-1); //flipando o sprite
                destinoPatrulha = 0;
            }
        }
    }
}
