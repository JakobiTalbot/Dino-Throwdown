﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    // stores original camera position
    private Vector3 m_v3OriginalCameraPosition;
    // stores shake strength (random range)
    private float m_fShakeStrength;
    // stores original shake strength
    private float m_fOriginalShakeStrength;
    // stores shake duration
    private float m_fShakeDuration;
    // stores time to shake camera
    private float m_fShakeTimer;
    // stores if camera is shaking or not
    private bool m_bShake = false;

	// Use this for initialization
	void Awake()
    {
        // get original camera transform
        m_v3OriginalCameraPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update()
    {
        // return if camera is not shaking
        if (!m_bShake)
            return;

        // decrement timer
        m_fShakeTimer -= Time.deltaTime;

        // camera has stopped shaking
        if (m_fShakeTimer <= 0.0f)
        {
            // reset camera position
            transform.position = m_v3OriginalCameraPosition;
            m_bShake = false;
        }
        else // camera is still shaking
        {
            // decrease screen shake strength over time
            m_fShakeStrength -= m_fOriginalShakeStrength / m_fShakeDuration * Time.deltaTime;

            // get offset
            Vector3 v3PosOffset = new Vector3
            {
                x = Random.Range(-m_fShakeStrength, m_fShakeStrength),
                y = Random.Range(-m_fShakeStrength, m_fShakeStrength)
            };

            // set offset position
            transform.position = m_v3OriginalCameraPosition + v3PosOffset;
        }
	}

    // gets screen shake data
    public void SetShake(float fShakeStrength, float fShakeDuration)
    {
        // set shake variables
        m_fShakeDuration = fShakeDuration;
        m_fOriginalShakeStrength = fShakeStrength;
        m_fShakeTimer = fShakeDuration;
        m_fShakeStrength = fShakeStrength;
        m_bShake = true;
    }
}