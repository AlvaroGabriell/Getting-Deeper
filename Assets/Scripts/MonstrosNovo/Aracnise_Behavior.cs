using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aracnise_Behavior : MonoBehaviour
{

    public float velocidade; //velocidade que Aracnise se movimenta
    public float qtd_Dano; //dano dado por esse monstro;
    public bool agressivo = false; //estado inicial da Aracnise
    private GameObject player; //pegar informações relacionadas ao jogador
    public Animator echoAnim; //controlar animator dAracnise e da Beth
    bool luz;
    public Transform spawnPoint; //ponto de descanso/retorno para Aracnise, controlado por um GameObject filho do sprite

    //Work in progress 05/07/25 - só fiz copiar os metodos do outro por hora

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
