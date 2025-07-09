using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AracniseAnimationEvents : MonoBehaviour
{
    public AracniseController controller;

    void Awake()
    {
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
}
