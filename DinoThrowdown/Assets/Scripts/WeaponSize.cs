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
            GameObject arm = other.GetComponent<PlayerMovement>().m_arm;
            // increases the scale of the object
            arm.transform.localScale = new Vector3(4.0f, 0.4f, 0.4f);
            // sets this object to inactive
            gameObject.SetActive(false);
        }
    }
}