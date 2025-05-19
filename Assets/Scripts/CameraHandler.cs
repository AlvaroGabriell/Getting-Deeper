using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Rigidbody2D rb;
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
        if (rb.velocity.x > 0.1)
        {
            targetScreenX = 0.5f;
        }
        else if (rb.velocity.x < -0.1)
        {
            targetScreenX = 0.87f;
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
