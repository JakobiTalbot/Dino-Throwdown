using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSize : Pickup
{
    private void OnTriggerEnter(Collider other)
    {
        // checks if the colliding object is a player and they don't already have a pickup
        if (other.CompareTag("Player") &&
            other.GetComponent<PlayerController>().m_weapon.transform.localScale.x < 4.0f &&
            other.GetComponent<PlayerController>().m_cruiseControl.bFlag != true &&
            other.GetComponent<Knockback>().m_shield.bFlag != true)
        {
            // gets the arm component from the player
            GameObject weapon = other.GetComponent<PlayerController>().m_weapon;
            // doubles the scale of the object
            weapon.transform.localScale = weapon.transform.localScale * 2.0f;
            // set position of weapon because of changed scale
            Vector3 v3ScaledPos = weapon.transform.localPosition;
            v3ScaledPos.x *= 2.0f;
            weapon.transform.localPosition = v3ScaledPos;
            // sets this object to inactive
            gameObject.SetActive(false);
            // empties the spawn point
            m_spawnPoint.m_bHasPickup = false;
        }
    }
}