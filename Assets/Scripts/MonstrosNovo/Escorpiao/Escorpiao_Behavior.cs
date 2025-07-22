using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Escorpiao_Behavior : MonoBehaviour
{
    public float velocidade; //velocidade que o Escorpião se movimenta
    public float qtd_Dano; //dano dado por esse monstro;
    public bool agressivo = false, aboveGround = false; //estado inicial do Escorpião e verificar se ele está acima ou abaixo do solo
    private GameObject player; //pegar informações relacionadas ao jogador
    public Animator scorpAnim; //controlar animator do Escorpião
    public Transform spawnPoint, abovePoint; //ponto de descanso/retorno para o Escorpião, controlado por um GameObject filho do sprite
    public CapsuleCollider2D colider;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        colider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        colider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (agressivo)
        {
            colider.enabled = true;
            //Fazer o sprite sair de baixo do solo e colocar-lo na animação de cavando para cima
            UnityEngine.Vector3 levantando = transform.position; //Manipulando a posição Y do sprite pra cima
            while (levantando.y <= abovePoint.position.y)
            {
                levantando.y += velocidade * Time.deltaTime;
                transform.position = levantando;
                scorpAnim.SetBool("aboveGround", true);
            }
            aboveGround = true;
        }
        if (agressivo && aboveGround)
        {
            atacarJogador();
        }
    }

    private void atacarJogador() //Escorpião no estado agressivo atacando o jogador
    {
        agressivo = true; aboveGround = true;
        transform.position = UnityEngine.Vector2.MoveTowards(transform.position, player.transform.position, velocidade * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lanterna"))
        { //Em contato com a luz ele se torna passivo
            agressivo = false; scorpAnim.SetBool("agressivo", false);
            colider.enabled = false;
            scorpAnim.Play("Sarcofago_Dig");
            scorpAnim.SetBool("aboveGround", false); aboveGround = false;
            UnityEngine.Vector3 descendo = transform.position; //Manipulando a posição Y do sprite pra cima
            while (descendo.y >= spawnPoint.position.y)
            {
                descendo.y -= velocidade * Time.deltaTime;
                transform.position = descendo;

            }
            
        }


        if (collision.CompareTag("Player"))
        {
            IDamageble damageble = collision.GetComponent<IDamageble>(); //tudo o que pode levar dano está inserido e detectavel com o IDamageble
            if (damageble != null)
            {//se algo recebeu dano
                damageble.Dano(qtd_Dano);
            }
        }

    }


}

