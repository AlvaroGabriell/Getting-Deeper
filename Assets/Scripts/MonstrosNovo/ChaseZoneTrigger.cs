using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseZoneTrigger : MonoBehaviour
{
    private AracniseController aracniseController;
    public bool isPlayerInChaseZone = false;

    void Awake()
    {
        aracniseController = transform.parent.GetComponentInChildren<AracniseController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInChaseZone = true;
            aracniseController.OnPlayerEnteredChaseZone();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInChaseZone = false;
            aracniseController.OnPlayerExitedChaseZone();
        }
    }
}
