﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBomb : MonoBehaviour
{
    // amount of force from the bomb
    public float m_fExplosionForce = 1000.0f;
    // reference to the players layer
    public LayerMask m_playerMask;
    // effect radius of the explosion
    public float m_fExplosionRadius = 5.0f;
    // delay before the bomb explodes
    public float m_fExplosionDelay = 1.5f;
    // reference to the explosion particle
    public ParticleSystem m_explosionParticle;
    // size of the particle
    public float m_fParticleSize = 1.3f;
    // reference to the explosion audio
    public AudioSource m_explosionAudio;
    // strength for the controller to vibrate when hit by explosion
    public float m_fVibrationStrength = 1.0f;
    // time for the controller to vibrate when hit by explosion
    public float m_fVibrationTime = 0.5f;
    // maximum amount of blocks that can be destroyed by one explosion
    public int m_nMaxBlocksToDestroy = 3;

    // reference to the claw that drops the bomb
    private Claw m_claw;
    // determines if vibration is on
    private bool m_bVibrationToggle = true;
    // stores current number of ground blocks destroyed
    private int m_nBlocksDestroyed = 0;

    private void Start()
    {
        m_claw = GetComponentInParent<Claw>();
        // gets whether vibration is on
        if (OptionsManager.InstanceExists)
        {
            m_bVibrationToggle = OptionsManager.Instance.m_bVibration;
        }
    }

    // find all the players in the area around the bomb and damage them
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Deadzone") || other.CompareTag("Claw") || other.CompareTag("Untagged"))
        {
            return;
        }

        // detatches the bomb from the claw
        if (m_claw)
        {
            m_claw.m_bHasItem = false;
            m_claw.m_bItemDrop = false;
        }
        gameObject.transform.parent = null;
        // destroys the bomb after the delay
        Destroy(gameObject, m_fExplosionDelay);
    }

    // calculates the amount of damage a target should take based on its position
    private float CalculateDamage(Vector3 targetPosition)
    {
        // vector from the target to the shell
        Vector3 explosionToTarget = targetPosition - transform.position;
        // magnitude of the vector
        float fExplosionDistance = explosionToTarget.magnitude;

        // the distance relative to the explosion (starts off at 1 at the centre and ends at 0 at the radius)
        float relativeDistance = (m_fExplosionRadius - fExplosionDistance) / m_fExplosionRadius;
        // multiplies the distance by the maximum damage
        float fDamage = relativeDistance/* * m_maxDamage*/;

        // ensures that the damage is not negative
        fDamage = Mathf.Max(0.0f, fDamage);

        return fDamage;
    }

    private void OnDestroy()
    {
        // adds all colliders that collide with a sphere of radius m_fExplosionRadius to an array
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_fExplosionRadius);
        // adds all players that collide with a sphere of radius m_fExplosionRadius to an array
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, m_fExplosionRadius, m_playerMask);

        // iterates through all colliders
        for (int i = 0; i < colliders.Length; i++)
        {
            // tries to get the rigidbody from the target
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            // checks if a rigidbody was not found
            if (!targetRigidbody)
            {
                continue;
            }

            // on collision with ground block
            if (colliders[i].CompareTag("Ground") && m_nBlocksDestroyed < m_nMaxBlocksToDestroy)
            {
                targetRigidbody.isKinematic = false;
                m_nBlocksDestroyed++;
            }
            else if (colliders[i].CompareTag("Player"))
            {
                // adds an explosion force to the target from the bomb
                targetRigidbody.AddExplosionForce(m_fExplosionForce + m_fExplosionForce * (colliders[i].GetComponent<Knockback>().m_fKnockbackMeter / 100.0f),
                                                  transform.position, m_fExplosionRadius);
                // calculates the damage to be dealt to the target
                float fDamage = CalculateDamage(targetRigidbody.position);
                colliders[i].GetComponent<Knockback>().m_fKnockbackMeter += fDamage * 20.0f;
            }
        }

        // creates an explosion of the specified size
        m_explosionParticle.gameObject.transform.parent = null;
        m_explosionParticle.transform.localScale *= m_fParticleSize;
        m_explosionParticle.Play();

        // gets the sfx volume from the options
        if (OptionsManager.InstanceExists)
        {
            m_explosionAudio.volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
        }
        // plays the explosion audio
        m_explosionAudio.Play();

        // vibrates the controllers of all the hit players
        if (m_bVibrationToggle)
        {
            for (int i = 0; i < playerColliders.Length; i++)
            {
                playerColliders[i].GetComponent<PlayerController>().SetVibration(m_fVibrationTime, m_fVibrationStrength);
            }
        }
    }
}