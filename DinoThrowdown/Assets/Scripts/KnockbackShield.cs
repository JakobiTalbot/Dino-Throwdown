using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackShield : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // checks if the colliding object is a player
        if (other.CompareTag("Player"))
        {
            // turns on the player's shield
            other.GetComponent<Knockback>().m_shield.bFlag = true;
            // sets this object to inactive
            gameObject.SetActive(false);
        }
    }
}