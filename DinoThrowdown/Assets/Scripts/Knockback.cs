﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float m_fKnockbackForce = 500.0f;
    public float m_fBaseKnockbackForce = 500.0f;
    public float m_fVelocityFactor = 100.0f;

    private Rigidbody m_rigidbody;
    private float m_fKnockbackMeter = 0;

    // Use this for initialization
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Find average position of two agents colliding
            Vector3 v3ExplosionPos = (collision.gameObject.transform.position + transform.position) * 0.5f;
            float fRelaVelForce = (collision.rigidbody.velocity - GetComponent<Rigidbody>().velocity).magnitude * m_fVelocityFactor;
            float fExplosionForce = (m_fKnockbackForce + fRelaVelForce) * (m_fKnockbackMeter / 100.0f) + m_fBaseKnockbackForce;
            m_rigidbody.AddExplosionForce(fExplosionForce, v3ExplosionPos, 5.0f);

            Debug.Log(fExplosionForce);

            m_fKnockbackMeter += fExplosionForce * 0.02f;

            // Clamp knockback meter
            if (m_fKnockbackMeter > 100.0f)
            {
                m_fKnockbackMeter = 100.0f;
            }
        }
    }

    public float GetKnockback()
    {
        return m_fKnockbackMeter;
    }
}