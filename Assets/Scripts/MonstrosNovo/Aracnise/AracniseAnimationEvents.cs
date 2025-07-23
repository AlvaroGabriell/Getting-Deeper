using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AracniseAnimationEvents : MonoBehaviour
{
    public AracniseController controller;
    GameObject player;
    float distance = 0;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        if (controller == null)
        {
            controller = GetComponentInParent<AracniseController>();
            if (controller == null)
            {
                Debug.LogError("AracniseController component not found in parent!");
            }
        }
    }

    public void FinalizarTransformacao()
    {
        controller.FinalizarTransformacao();
    }

    public void PlayAttackSFX()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= 20)
        {
            SFXManager.Instance.PlaySFX("AracniseAttack");
        }
    }
    public void PlayStepsSFX()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= 20)
        {
            SFXManager.Instance.PlaySFX("AracniseSteps");
        }
    }
    public void PlayTransformSFX()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= 20)
        {
            SFXManager.Instance.PlaySFX("AracniseTransform");
        }
    }
}
