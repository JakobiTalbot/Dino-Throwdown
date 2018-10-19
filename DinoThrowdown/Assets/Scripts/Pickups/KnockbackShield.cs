using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackShield : Pickup
{
    public GameObject m_pickupParticles;
    public float m_fRotateSpeed = 10.0f;
    public float m_fBobSpeed = 1.0f;
    public float m_fBobAmount = 0.2f;

    private float m_fOriginalY;
    private float m_fSineCounter = 0.0f;

    private void Awake()
    {
        m_fOriginalY = transform.localPosition.y;
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
            // sets this object to inactive
            gameObject.SetActive(false);
            // empties the spawn point
            m_spawnPoint.m_bHasPickup = false;

            // create pickup particles
            Instantiate(m_pickupParticles, transform.position, Quaternion.Euler(0, 0, 0));
        }
    }
}