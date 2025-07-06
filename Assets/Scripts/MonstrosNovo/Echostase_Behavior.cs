using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echostase_Behavior : MonoBehaviour
{
    public float velocidade; //velocidade que o Echostase se movimenta
    public float qtd_Dano; //dano dado por esse monstro;
    public bool agressivo = false; //estado inicial do Echostase
    private GameObject player; //pegar informações relacionadas ao jogador
    public Animator echoAnim; //controlar animator do Echostase e da Beth
    bool agachando, rastejando; //pegando estados de movimento da Beth
    public Transform spawnPoint; //ponto de descanso/retorno para o Echostase, controlado por um GameObject filho do sprite


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            //Debug.Log("EEE");
            echoAnim.SetBool("idle", true);
            echoAnim.SetBool("jogadorDetectado", false);
            return;
        }
        if (agressivo == true)
        {
            atacarJogador();
        }
        else
        {
            //Se o jogador não está mais em alcance,está agachado ou rastejando, retornar à posição inicial
            retornarDescanso();
        }
        flipSprite();
    }

    private void atacarJogador() //Ao passar pelo echostase sem estar num "tipo de movimento seguro" ele se tornará agressivo
    {
        //Debug.Log("DDD");
        echoAnim.SetBool("idle", false);
        echoAnim.SetBool("jogadorDetectado", true);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, (velocidade * Time.deltaTime));
    }

    private void retornarDescanso()
    {
        echoAnim.SetBool("jogadorDetectado", false);
        transform.position = Vector2.MoveTowards(transform.position, spawnPoint.position, velocidade * Time.deltaTime);
        if (transform.position.x == spawnPoint.position.x)
        {
            echoAnim.SetBool("idle", true);
        }
    }
    private void flipSprite() //Rotacionar o sprite dependendo da posição do jogador
    {
        if (transform.position.x > player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
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
