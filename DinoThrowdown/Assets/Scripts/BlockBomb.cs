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
    // reference to the explosion
    public GameObject m_explosion;
    // size of the particle
    public float m_fParticleSize = 1.3f;
    // strength for the controller to vibrate when hit by explosion
    public float m_fVibrationStrength = 1.0f;
    // time for the controller to vibrate when hit by explosion
    public float m_fVibrationTime = 0.5f;
    // maximum amount of blocks that can be destroyed by one explosion
    public int m_nMaxBlocksToDestroy = 3;
    // strength to shake the screen (random range)
    public float m_fScreenShakeStrength = 1.0f;
    // time to shake the screen for in seconds
    public float m_fScreenShakeDuration = 1.0f;

    [HideInInspector]
    public GameObject m_bombDropper;

    // stores reference to game camera
    private GameObject m_camera;
    // stores whether the application is quitting or not
    private bool m_bAppQuitting = false;
    // determines if vibration is on
    private bool m_bVibrationToggle = true;
    // stores current number of ground blocks destroyed
    private int m_nBlocksDestroyed = 0;
    // stores the round the current bomb was spawned
    private int m_nRoundSpawned;
    private float m_fOriginalVolume = 1.0f;

    private void Awake()
    {
        // gets the original volume from the explosion prefab
        m_fOriginalVolume = m_explosion.GetComponent<AudioSource>().volume;
    }

    private void Start()
    {
        GetComponent<Collider>().material.bounceCombine = PhysicMaterialCombine.Minimum;
        // gets whether vibration is on
        if (OptionsManager.InstanceExists)
        {
            m_bVibrationToggle = OptionsManager.Instance.m_bVibration;
        }
        m_nRoundSpawned = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameStateManager>().m_nRoundNumber;
        // find camera object
        m_camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // find all the players in the area around the bomb and damage them
    private void OnTriggerEnter(Collider other)
    {
        // checks if the bomb didn't collide with a relevant surface and exits if so
        if (other.CompareTag("Deadzone") || other.CompareTag("Claw") || other.CompareTag("Untagged") || transform.parent)
        {
            return;
        }
        // unparents the bomb from the claw
        gameObject.transform.parent = null;
        // destroys the bomb after the delay
        GetComponent<Animator>().SetTrigger("startFlashing");
        Destroy(gameObject, m_fExplosionDelay);
    }

    // calculates the amount of damage a target should take based on its position
    private float CalculateDamage(Vector3 v3TargetPosition)
    {
        // vector from the target to the shell
        Vector3 v3ExplosionToTarget = v3TargetPosition - transform.position;
        // magnitude of the vector
        float fExplosionDistance = v3ExplosionToTarget.magnitude;

        // the distance relative to the explosion (starts off at 1 at the centre and ends at 0 at the radius)
        float fRelativeDistance = (m_fExplosionRadius - fExplosionDistance) / m_fExplosionRadius;

        // ensures that the damage is not negative
        fRelativeDistance = Mathf.Max(0.0f, fRelativeDistance);

        return fRelativeDistance;
    }

    // applys damage to all players in the explosion radius when the bomb is being destroyed
    private void OnDestroy()
    {
        if (!m_bAppQuitting && m_nRoundSpawned == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameStateManager>().m_nRoundNumber)
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

            // create explosion at bomb position if application is not quitting
            GameObject explosion = Instantiate(m_explosion, transform.position, transform.rotation);
            explosion.transform.localScale *= m_fParticleSize;

            // do screen shake
            m_camera.GetComponent<ScreenShake>().SetShake(m_fScreenShakeStrength, m_fScreenShakeDuration);

            // gets the sfx volume from the options
            if (OptionsManager.InstanceExists)
            {
                explosion.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume * m_fOriginalVolume;
            }

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

    private void OnApplicationQuit()
    {
        m_bAppQuitting = true;
    }
}