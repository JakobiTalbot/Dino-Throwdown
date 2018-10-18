using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackShield : Pickup
{
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
        }
    }
}