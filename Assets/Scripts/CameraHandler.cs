using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public PlayerController playerController;
    public CinemachineVirtualCamera cam;
    private CinemachineFramingTransposer transposer;

    public float smoothTime = 0.4f; //Smaller = faster
    private float currentVelocity, targetScreenX = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        transposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.GetAimDirection().x > 0)
        {
            targetScreenX = 0.5f; // Aim right
        }
        else if(playerController.GetAimDirection().x < 0)
        {
            targetScreenX = 1.0f; // Aim left
        }
        else
        {
            targetScreenX = 0.5f; // Fallback
        }

        // Smoothly interpolate the screenX value
        transposer.m_ScreenX = Mathf.SmoothDamp(
            transposer.m_ScreenX,
            targetScreenX,
            ref currentVelocity,
            smoothTime
        );
    }
}
