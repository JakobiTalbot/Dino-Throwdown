using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSize : Pickup
{
    public GameObject m_pickupParticles;
    public float m_fRotateSpeed = 10.0f;
    public float m_fBobSpeed = 1.0f;
    public float m_fBobAmount = 0.2f;

    private float m_fOriginalY;
    private float m_fSineCounter = 0.0f;
    private float m_fOriginalVolume = 1.0f;

    private void Awake()
    {
        m_fOriginalY = transform.localPosition.y;

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
            // gets the arm component from the player
            GameObject weapon = other.GetComponent<PlayerController>().m_weapon;
            // doubles the scale of the object
            weapon.transform.localScale = weapon.transform.localScale * 2.0f;
            other.GetComponent<PlayerController>().m_weaponSize.bFlag = true;
            other.GetComponent<PlayerController>().m_weaponSize.fTimer = other.GetComponent<PlayerController>().m_fWeaponSizeTime;
            // set position of weapon because of changed scale
            Vector3 v3ScaledPos = weapon.transform.localPosition;
            v3ScaledPos.x *= 2.0f;
            weapon.transform.localPosition = v3ScaledPos;
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
            // increases the claw's size
            other.GetComponent<Claw>().m_weaponSize.bFlag = true;
            other.GetComponent<Claw>().m_weaponSize.fTimer = other.GetComponent<Claw>().m_fWeaponSizeTime;
            other.transform.localScale *= other.GetComponent<Claw>().m_fWeaponSizeMultiplier;
            // increases the line's size
            other.GetComponent<Claw>().m_fLineLength /= other.GetComponent<Claw>().m_fWeaponSizeMultiplier;
            other.GetComponentInChildren<LineRenderer>().widthMultiplier *= other.GetComponent<Claw>().m_fWeaponSizeMultiplier;

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