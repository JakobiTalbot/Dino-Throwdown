using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiseControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // checks if the colliding object is a player
        if (other.CompareTag("Player"))
        {
            // turns on the player's cruise control
            other.GetComponent<PlayerController>().m_cruiseControl.bFlag = true;
            // sets this object to inactive
            gameObject.SetActive(false);
        }
    }
}