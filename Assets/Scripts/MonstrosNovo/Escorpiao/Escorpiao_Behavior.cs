using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escorpiao_Behavior : MonoBehaviour
{
    public float velocidade; //velocidade que o Escorpião se movimenta
    public float qtd_Dano; //dano dado por esse monstro;
    public bool agressivo = false, aboveGround = false; //estado inicial do Escorpião e verificar se ele está acima ou abaixo do solo
    private GameObject player; //pegar informações relacionadas ao jogador
    public Animator scorpAnim; //controlar animator do Escorpião
    public Transform spawnPoint; //ponto de descanso/retorno para o Escorpião, controlado por um GameObject filho do sprite


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (agressivo && aboveGround){
            wakeUp();
        }
    }

    void wakeUp(){ //Fazer o sprite sair de baixo do solo e colocar-lo na animação de cavando para cima

    }
}
