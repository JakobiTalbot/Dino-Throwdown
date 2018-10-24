using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    // collection of different pickup types
    public GameObject[] m_pickupTypes;
    // time between spawning
    public float m_fSpawnTime = 10.0f;
    // collection of spawn locations
    public Transform[] m_spawnPoints;

    private void Start()
    {
        if (OptionsManager.InstanceExists)
        {
            if (OptionsManager.Instance.m_bPickups)
            {
                // repeatedly calls the Spawn function
                InvokeRepeating("Spawn", m_fSpawnTime, m_fSpawnTime);
            }
        }
        else
        {
            // repeatedly calls the Spawn function
            InvokeRepeating("Spawn", m_fSpawnTime, m_fSpawnTime);
        }
    }

    // spawns a pickup at a random location
    private void Spawn()
    {
        // random location index
        int iSpawnPointIndex = Random.Range(0, m_spawnPoints.Length);
        // copy of the index minus 1
        int iCopy = iSpawnPointIndex - 1;
        // wraps around
        if (iCopy == -1)
        {
            iCopy = m_spawnPoints.Length - 1;
        }

        // random pickup type index
        int iPickupTypeIndex = Random.Range(0, m_pickupTypes.Length);

        // loops until a spawn point does not have a pickup
        while (m_spawnPoints[iSpawnPointIndex].GetComponent<SpawnPoint>().m_bHasPickup)
        {
            // checks if a full loop occured
            if (iSpawnPointIndex == iCopy)
            {
                break;
            }
            else
            {
                // increments the index
                iSpawnPointIndex++;
                // wraps around
                if (iSpawnPointIndex == m_spawnPoints.Length)
                {
                    iSpawnPointIndex = 0;
                }
            }
        }

        // gets the spawn point component
        SpawnPoint spawnPoint = m_spawnPoints[iSpawnPointIndex].GetComponent<SpawnPoint>();

        // checks if the spawn point has a pickup
        if (!spawnPoint.m_bHasPickup)
        {
            // creates the random pickup at the random location
            GameObject obj = Instantiate(m_pickupTypes[iPickupTypeIndex], m_spawnPoints[iSpawnPointIndex].position, m_spawnPoints[iSpawnPointIndex].rotation);
            // gets the pickup component from the instantiated object
            Pickup pickup = obj.GetComponent<Pickup>();
            // sets the pickup's spawn point
            pickup.m_spawnPoint = spawnPoint;
            // sets the spawn point pickup status to true
            spawnPoint.m_bHasPickup = true;
        }
    }
}