using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallKnockback : MonoBehaviour
{
    // amount of knockback force
    public float m_fKnockbackForce = 1000.0f;
    // knockback meter contribution multiplier
    public float m_fForceToKnockbackMeter = 0.1f;
    // velocity contribution multiplier
    public float m_fVelocityFactor = 100.0f;
    // vibration time
    public float m_fVibrateTimeWhenHit = 0.3f;
    // vibration strength
    public float m_fVibrateStrengthWhenHit = 0.5f;
    // offsets the y position of the explosion
    public float m_fExplosionYPosOffset = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        // stores the knockback script from the collider if possible
        Knockback player;

        // trys to get the knockback script from the collider
        if (other.GetComponent<Knockback>() == null)
        {
            if (other.GetComponentInParent<Knockback>() == null)
            {
                return;
            }
            else
            {
                player = other.GetComponentInParent<Knockback>();
            }
        }
        else
        {
            player = other.GetComponent<Knockback>();
        }

        // does not apply knock back if the player is being picked up
        if (player.GetComponent<Rigidbody>().isKinematic)
        {
            return;
        }

        // Find average position of two objects colliding
        Vector3 v3ExplosionPos = (player.transform.position + transform.position) * 0.5f;
        // moves the explosion position by the offset
        v3ExplosionPos.y -= m_fExplosionYPosOffset;
        // determines the force from the velocity
        float fRelaVelForce = (player.GetComponent<Rigidbody>().velocity - GetComponent<Rigidbody>().velocity).magnitude * m_fVelocityFactor;
        // the magnitude of the explosion
        float fExplosionForce = (m_fKnockbackForce + fRelaVelForce) * (player.m_fKnockbackMeter / 100.0f) + m_fKnockbackForce;

        // checks if the player's shield is on
        if (player.m_shield.bFlag)
        {
            // halves the force
            fExplosionForce /= 2.0f;
        }

        // adds the knockback to the player
        player.GetComponent<Rigidbody>().AddExplosionForce(fExplosionForce, v3ExplosionPos, 5.0f);

        // increases the player's knockback meter
        player.m_fKnockbackMeter += fExplosionForce * m_fForceToKnockbackMeter;

        // clamps the knockback meter
        if (player.m_fKnockbackMeter > 100.0f)
        {
            player.m_fKnockbackMeter = 100.0f;
        }

        // vibrate player's controller
        player.GetComponent<PlayerController>().SetVibration(m_fVibrateTimeWhenHit, m_fVibrateStrengthWhenHit);

        player.GetComponent<PlayerController>().m_fKnockedBackTimer = player.GetComponent<PlayerController>().m_fStopCruiseAfterHitTime;
        // creates hit particles
        Instantiate(player.m_hitParticles, v3ExplosionPos, Quaternion.Euler(0, 0, 0));
    }
}