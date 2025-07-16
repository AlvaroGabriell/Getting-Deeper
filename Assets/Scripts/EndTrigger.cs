using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndTrigger : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<UIHandler>().PlayEndSequence();
            player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
        }
    }
}