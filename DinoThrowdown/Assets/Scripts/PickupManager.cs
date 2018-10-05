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
        // repeatedly calls the Spawn function
        InvokeRepeating("Spawn", m_fSpawnTime, m_fSpawnTime);
    }

    // spawns a pickup at a random location
    private void Spawn()
    {
        // random location index
        int iSpawnPointIndex = Random.Range(0, m_spawnPoints.Length);
        // random pickup type index
        int iPickupTypeIndex = Random.Range(0, m_pickupTypes.Length);
        // creates the random pickup at the random location
        Instantiate(m_pickupTypes[iPickupTypeIndex], m_spawnPoints[iSpawnPointIndex].position, m_spawnPoints[iSpawnPointIndex].rotation);
    }
}