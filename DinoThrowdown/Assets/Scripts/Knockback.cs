using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Knockback : MonoBehaviour
{
    // used to reduce knockback for a limited time
    public struct KnockbackShield
    {
        public bool bFlag;
        public float fTimer;
    }

    public float m_fWeaponKnockbackForce = 500.0f;
    public float m_fBaseWeaponKnockbackForce = 500.0f;

    public float m_fForceToKnockbackMeter = 0.01f;

    public float m_fVelocityFactor = 100.0f;
    public float m_fVibrateTime = 0.5f;
    public float m_fKnockbackShieldTime = 5.0f;

    public GameObject m_hitParticles;

    // handles when the player should recieve less knockback
    [HideInInspector]
    public KnockbackShield m_shield;
    [HideInInspector]
    public float m_fKnockbackMeter = 0;


    private Rigidbody m_rigidbody;
    // used to check if being hit by a larger weapon
    private Vector3 m_v3Larger;
    private float m_fVibrateTimer;
    private bool m_bIsVibrating;

    // Use this for initialization
    void Awake()
    {
        m_fVibrateTimer = m_fVibrateTime;
        m_rigidbody = GetComponent<Rigidbody>();

        m_shield.bFlag = false;
        m_shield.fTimer = 5.0f;

        m_v3Larger = new Vector3(4.0f, 0.4f, 0.4f);
	}

    private void Update()
    {
        // checks if the shield is on
        if (m_shield.bFlag)
        {
            m_shield.fTimer -= Time.deltaTime;
            if (m_shield.fTimer <= 0.0f)
            {
                // resets the shield
                m_shield.bFlag = false;
                m_shield.fTimer = m_fKnockbackShieldTime;
            }
        }

        if (m_bIsVibrating)
        {
            // decrement vibration timer
            m_fVibrateTimer -= Time.deltaTime;
            if (m_fVibrateTimer <= 0.0f)
            {
                m_bIsVibrating = false;
                GamePad.SetVibration((PlayerIndex)GetComponent<PlayerController>().m_cPlayerNumber - 1, 0.0f, 0.0f);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && !CompareTag("Weapon")
            && other.gameObject.GetComponentInParent<Component>().GetComponentInParent<Component>().GetComponentInParent<PlayerController>().GetAttacking() 
            && !other.gameObject.GetComponentInParent<Component>().GetComponentInParent<Component>().GetComponentInParent<PlayerController>().m_bWeaponHit)
        {
            // Find average position of two objects colliding
            Vector3 v3ExplosionPos = (other.gameObject.transform.position + transform.position) * 0.5f;
            v3ExplosionPos.y += 0.5f;
            float fRelaVelForce = (other.gameObject.GetComponentInParent<Component>().GetComponentInParent<Component>().GetComponentInParent<Rigidbody>().velocity - GetComponent<Rigidbody>().velocity).magnitude * m_fVelocityFactor;
            float fExplosionForce = (m_fWeaponKnockbackForce + fRelaVelForce) * (m_fKnockbackMeter / 100.0f) + m_fBaseWeaponKnockbackForce;

            // checks if the object is a weapon with the increase size pickup
            if (other.gameObject.transform.localScale == m_v3Larger)
            {
                // doubles the force
                fExplosionForce *= 2.0f;
            }

            // checks if the shield is on
            if (m_shield.bFlag)
            {
                // halves the force
                fExplosionForce /= 2.0f;
            }

            // do knockback
            m_rigidbody.AddExplosionForce(fExplosionForce, v3ExplosionPos, 5.0f);

            // increase knockback meter
            m_fKnockbackMeter += fExplosionForce * m_fForceToKnockbackMeter;

            // clamp knockback meter
            if (m_fKnockbackMeter > 100.0f)
            {
                m_fKnockbackMeter = 100.0f;
            }

            // vibrate player's controller
            if (!m_bIsVibrating)
            {
                GamePad.SetVibration((PlayerIndex)GetComponent<PlayerController>().m_cPlayerNumber - 1, 1.0f, 1.0f);
                m_fVibrateTimer = m_fVibrateTime;
                m_bIsVibrating = true;
            }

            other.gameObject.GetComponentInParent<Component>().GetComponentInParent<Component>().GetComponentInParent<PlayerController>().m_bWeaponHit = true;
            GetComponent<PlayerController>().m_fKnockedBackTimer = GetComponent<PlayerController>().m_fStopCruiseAfterHitTime;
            Instantiate(m_hitParticles, v3ExplosionPos, Quaternion.Euler(0, 0, 0));
        }
    }

    public float GetKnockback()
    {
        return m_fKnockbackMeter;
    }
}