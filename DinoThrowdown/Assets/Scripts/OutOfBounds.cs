using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    // reference to the crane
    public CraneManager[] m_crane;
    // reference to the crane seats
    public Transform[] m_seats;

    // reference to the claw
    private Claw[] m_claw;

    private void Start()
    {
        m_claw = new Claw[2];

        // gets and sets the claw component from each crane
        for (int i = 0; i < m_crane.Length; i++)
        {
            m_claw[i] = m_crane[i].GetComponentInChildren<Claw>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if the collider is a player
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            // iterates through each crane
            for (int i = 0; i < m_crane.Length; i++)
            {
                // checks if the crane is occupied
                if (!m_crane[i].m_bOccupied)
                {
                    // sets the crane to occupied
                    m_crane[i].m_bOccupied = true;
                    m_crane[i].m_player = playerController.gameObject;

                    // gets the claw's light
                    Light light = m_claw[i].GetComponentInChildren<Light>();
                    light.intensity = 500.0f;

                    // sets the colour of the light to the colour of the player
                    switch (playerController.m_cPlayerNumber)
                    {
                        case 1:
                            light.color = Color.cyan;
                            break;
                        case 2:
                            light.color = Color.red;
                            break;
                        case 3:
                            light.color = Color.green;
                            break;
                        case 4:
                            light.color = Color.yellow;
                            break;
                    }

                    // sends the player to the crane
                    playerController.transform.position = m_seats[i].transform.position;

                    playerController.transform.localRotation = Quaternion.Euler(0.0f, m_seats[i].transform.rotation.eulerAngles.y, 0.0f);

                    // changes the status of the player to in the crane
                    playerController.m_bInCrane = true;
                    // gives the player control of the claw
                    playerController.m_claw = m_claw[i];

                    other.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
                    other.GetComponent<Rigidbody>().isKinematic = true;

                    return;
                }
            }

            // sets the player as out if all cranes are occupied
            playerController.m_bIsOut = true;
        }
    }
}