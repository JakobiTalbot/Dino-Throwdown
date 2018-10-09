using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSize : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // checks if the colliding object is a player
        if (other.CompareTag("Player"))
        {
            // gets the arm component from the player
            GameObject arm = other.GetComponent<PlayerController>().m_arm;
            // doubles the scale of the object
            arm.transform.localScale = arm.transform.localScale * 2.0f;
            // sets this object to inactive
            gameObject.SetActive(false);
        }
    }
}