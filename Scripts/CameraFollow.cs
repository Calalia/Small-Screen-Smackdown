﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform PlayerTransform;

    private Vector3 cameraOffset;


    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;


    public bool LookAtPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraOffset = transform.position - PlayerTransform.position;
   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPos = PlayerTransform.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if(LookAtPlayer)
        {
            transform.LookAt(PlayerTransform);
        }
    }
}
