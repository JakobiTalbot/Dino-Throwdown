using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackShield : Pickup
{
    private void OnTriggerEnter(Collider other)
    {
        // checks if the colliding object is a player and they don't already have a pickup
        if (other.CompareTag("Player") &&
            other.GetComponent<PlayerController>().m_arm.transform.localScale.x < 4.0f &&
            other.GetComponent<PlayerController>().m_cruiseControl.bFlag != true &&
            other.GetComponent<Knockback>().m_shield.bFlag != true)
        {
            // turns on the player's shield
            other.GetComponent<Knockback>().m_shield.bFlag = true;
            // sets this object to inactive
            gameObject.SetActive(false);
            // empties the spawn point
            m_spawnPoint.m_bHasPickup = false;
        }
    }
}