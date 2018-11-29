using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackShield : Pickup
{
    // reference to the pickup particle to be instantiated
    public GameObject m_pickupParticles;
    // speed at which the object rotates
    public float m_fRotateSpeed = 10.0f;
    // speed at which the object bobs up and down
    public float m_fBobSpeed = 1.0f;
    // the amount the object bobs
    public float m_fBobAmount = 0.2f;

    // original y position of the object
    private float m_fOriginalY;
    // used to time bobbing
    private float m_fSineCounter = 0.0f;
    private float m_fOriginalVolume = 1.0f;

    private void Awake()
    {
        m_fOriginalY = transform.localPosition.y;
        // gets the original volume of the audio source attached to the pickup particle
        m_fOriginalVolume = m_pickupParticles.GetComponent<AudioSource>().volume;
    }

    void Update()
    {
        // rotate transform
        transform.Rotate(new Vector3(0.0f, m_fRotateSpeed, 0.0f) * Time.deltaTime);

        // increment sine counter
        m_fSineCounter += Time.deltaTime * m_fBobSpeed;

        // bobbing
        Vector3 v3Pos = transform.localPosition;
        v3Pos.y = m_fOriginalY + (Mathf.Sin(m_fSineCounter) * m_fBobAmount);
        transform.localPosition = v3Pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if the colliding object is a player and they don't already have a pickup
        if (other.CompareTag("Player") &&
            other.GetComponent<PlayerController>().m_weaponSize.bFlag != true &&
            other.GetComponent<PlayerController>().m_cruiseControl.bFlag != true &&
            other.GetComponent<Knockback>().m_shield.bFlag != true)
        {
            // turns on the player's shield
            other.GetComponent<Knockback>().m_shield.bFlag = true;
            other.GetComponent<Knockback>().m_shield.fTimer = other.GetComponent<Knockback>().m_fKnockbackShieldTime;
            other.GetComponent<Knockback>().m_shieldSphere.SetActive(true);
            // sets this object to inactive
            gameObject.SetActive(false);
            // empties the spawn point
            m_spawnPoint.m_bHasPickup = false;

            if (OptionsManager.InstanceExists)
            {
                m_pickupParticles.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fMasterVolume * OptionsManager.Instance.m_fSFXVolume * m_fOriginalVolume;
            }

            // create pickup particles
            GameObject newParticles = Instantiate(m_pickupParticles, other.transform.position, Quaternion.Euler(0, 0, 0));
            newParticles.transform.parent = other.transform;
        }
        // checks if the object is a claw without a pickup
        else if (other.CompareTag("Claw") &&
                 other.GetComponent<Claw>().m_weaponSize.bFlag != true &&
                 other.GetComponent<Claw>().m_cruiseControl.bFlag != true &&
                 other.GetComponent<Claw>().m_knockbackShield.bFlag != true)
        {
            // turns on the claw's knockback shield
            other.GetComponent<Claw>().m_knockbackShield.bFlag = true;
            other.GetComponent<Claw>().m_knockbackShield.fTimer = other.GetComponent<Claw>().m_fKnockbackShieldTime;

            // sets this object to inactive
            gameObject.SetActive(false);
            // empties the spawn point
            m_spawnPoint.m_bHasPickup = false;

            if (OptionsManager.InstanceExists)
            {
                m_pickupParticles.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fMasterVolume * OptionsManager.Instance.m_fSFXVolume * m_fOriginalVolume;
            }

            // create pickup particles
            GameObject newParticles = Instantiate(m_pickupParticles, other.transform.position, Quaternion.Euler(0, 0, 0));
            newParticles.transform.parent = other.transform;
        }
    }
}