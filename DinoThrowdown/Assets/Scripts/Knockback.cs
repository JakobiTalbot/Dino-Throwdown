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
    public float m_fVibrateTimeWhenHit = 0.5f;
    public float m_fVibrateStrengthWhenHit = 1.0f;
    public float m_fVibrateTimeWhenHitting = 0.2f;
    public float m_fVibrateStrengthWhenHitting = 1.0f;
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
    // reference to the hit music
    private AudioSource m_hitSound;

    // Use this for initialization
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_shield.bFlag = false;
        m_shield.fTimer = 5.0f;

        m_v3Larger = new Vector3(4.0f, 0.4f, 0.4f);

        m_hitSound = m_hitParticles.GetComponent<AudioSource>();
        if (OptionsManager.InstanceExists)
        {
            m_hitSound.volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
        }
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
                GetComponent<PlayerController>().m_shieldSphere.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && GetComponent<MeshCollider>()
            && other.gameObject.GetComponentInParent<Component>().GetComponentInParent<Component>().GetComponentInParent<PlayerController>().GetAttacking() 
            && !other.gameObject.GetComponentInParent<Component>().GetComponentInParent<Component>().GetComponentInParent<PlayerController>().m_bWeaponHit
            && !GetComponent<Rigidbody>().isKinematic)
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
            GetComponent<PlayerController>().SetVibration(m_fVibrateTimeWhenHit, m_fVibrateStrengthWhenHit);
            // vibrate other player's controller
            other.gameObject.GetComponentInParent<PlayerController>().SetVibration(m_fVibrateTimeWhenHitting, m_fVibrateStrengthWhenHitting);

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