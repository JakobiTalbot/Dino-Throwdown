using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    // used to reduce knockback for a limited time
    public struct KnockbackShield
    {
        public bool bFlag;
        public float fTimer;
    }

    public float m_fKnockbackForce = 500.0f;
    public float m_fBaseKnockbackForce = 500.0f;
    public float m_fVelocityFactor = 100.0f;
    // handles when the player should recieve less knockback
    [HideInInspector]
    public KnockbackShield m_shield;

    private Rigidbody m_rigidbody;
    private float m_fKnockbackMeter = 0;

    // Use this for initialization
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_shield.bFlag = false;
        m_shield.fTimer = 5.0f;
	}

    private void OnCollisionEnter(Collision collision)
    {
        // checks if the collision is with a player or an arm
        if (collision.gameObject.tag == "Player" || collision.gameObject.CompareTag("Arm"))
        {
            // Find average position of two agents colliding
            Vector3 v3ExplosionPos = (collision.gameObject.transform.position + transform.position) * 0.5f;
            v3ExplosionPos.y += 0.1f;
            float fRelaVelForce = (collision.rigidbody.velocity - GetComponent<Rigidbody>().velocity).magnitude * m_fVelocityFactor;
            float fExplosionForce = (m_fKnockbackForce + fRelaVelForce) * (m_fKnockbackMeter / 100.0f) + m_fBaseKnockbackForce;

            // checks if the shield is on
            if (m_shield.bFlag)
            {
                // halves the force
                fExplosionForce /= 2.0f;

                m_shield.fTimer -= Time.deltaTime;
                if (m_shield.fTimer <= 0.0f)
                {
                    // resets the shield
                    m_shield.bFlag = false;
                    m_shield.fTimer = 5.0f;
                }
            }

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