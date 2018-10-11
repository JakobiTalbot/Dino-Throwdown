using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    // reference to the crane
    public CraneOccupied m_crane;
    // reference to the claw
    public GameObject m_claw;

    private void OnTriggerEnter(Collider other)
    {
        // checks if the collider is a player
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (!m_crane.m_bOccupied)
            {
                // sets the crane to occupied
                m_crane.m_bOccupied = true;
                m_crane.m_player = playerController.gameObject;

                // activates the claw
                m_claw.SetActive(true);

                // sets the colour of the light to the colour of the player
                switch (playerController.m_cPlayerNumber)
                {
                    case 1:
                        m_claw.GetComponentInChildren<Light>().color = Color.cyan;
                        break;
                    case 2:
                        m_claw.GetComponentInChildren<Light>().color = Color.red;
                        break;
                    case 3:
                        m_claw.GetComponentInChildren<Light>().color = Color.green;
                        break;
                    case 4:
                        m_claw.GetComponentInChildren<Light>().color = Color.yellow;
                        break;
                }

                // sends the player to the crane
                playerController.transform.position = new Vector3(-22.5f, 7.3f, 31.5f);
                playerController.transform.localRotation = Quaternion.Euler(0.0f, 144.0f, 0.0f);
                // changes the status of the player to in the crane
                playerController.m_bInCrane = true;
                // gives the player control of the claw
                playerController.m_claw = m_claw;
            }
            else
            {
                // sets the player as out
                playerController.m_bIsOut = true;
            }
        }
    }
}