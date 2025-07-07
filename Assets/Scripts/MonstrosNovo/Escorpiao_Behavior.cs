using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escorpiao_Behavior : MonoBehaviour
{
    public float velocidade; //velocidade que o Escorpião se movimenta
    public float qtd_Dano; //dano dado por esse monstro;
    public bool agressivo = false; //estado inicial do Escorpião
    private GameObject player; //pegar informações relacionadas ao jogador
    public Animator echoAnim; //controlar animator do Escorpião
    public Transform spawnPoint; //ponto de descanso/retorno para o Escorpião, controlado por um GameObject filho do sprite


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
