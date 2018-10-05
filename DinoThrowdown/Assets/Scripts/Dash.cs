using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public float m_fDashForce = 1000.0f;
    public float m_fDashCooldown = 5.0f;

    private Rigidbody m_rigidbody;
    private float m_fTimer;
    private bool m_bCanDash = true;

	// Use this for initialization
	void Awake()
    {
        m_fTimer = m_fDashCooldown;
        m_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
    {
        // If dash cooldown is on
		if (!m_bCanDash)
        {
            // Increment timer
            m_fTimer -= Time.deltaTime;

            // Reset timer and allow dash after dash cooldown
            if (m_fTimer <= 0.0f)
            {
                m_bCanDash = true;
                m_fTimer = m_fDashCooldown;
            }
        }
	}

    public void DoDash()
    {
        if (m_bCanDash)
        {
            // Get position for explosive force
            Vector3 v3ExplosionPos = transform.position;
            v3ExplosionPos.x -= Input.GetAxis("Horizontal");
            v3ExplosionPos.y = transform.position.y + 0.7f; // So the player doesn't fly
            v3ExplosionPos.z -= Input.GetAxis("Vertical");

            m_rigidbody.AddExplosionForce(m_fDashForce, v3ExplosionPos, 20.0f);
            m_bCanDash = false;
        }
    }

    public float GetCooldown()
    {
        return m_fTimer;
    }
}