using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public float m_fForce = 1000.0f;

    private GameObject[] m_enemyObjects;
    private GameObject m_targetObject;
    private Rigidbody m_rigidbody;

    // Use this for initialization
    void Awake()
    {
        m_enemyObjects = GameObject.FindGameObjectsWithTag("Player");
        m_targetObject = m_enemyObjects[0];
        m_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
    {
        m_enemyObjects = GameObject.FindGameObjectsWithTag("Player");
        // Find closest enemy
        for (int i = 0; i < m_enemyObjects.Length; ++i)
        {
            if (m_enemyObjects[i] 
                && m_enemyObjects[i].transform.position.sqrMagnitude - transform.position.sqrMagnitude
                < m_targetObject.transform.position.sqrMagnitude - transform.position.sqrMagnitude
                && m_enemyObjects[i] != gameObject)
            {
                m_targetObject = m_enemyObjects[i];
            }
        }

        // Get direction to enemy
        Vector3 v3Dir = m_targetObject.transform.position - transform.position;
        // Seek ahead of enemy to intercept FIX THIS
        //v3Dir += m_targetObject.GetComponent<Rigidbody>().velocity * 2;
        v3Dir.y = 0.0f; // Nullify y force so agent doesn't fly when falling off edge

        if (v3Dir.sqrMagnitude > 1)
            v3Dir.Normalize();

        // Add force to rigidbody
        m_rigidbody.AddForce(v3Dir * m_fForce * Time.deltaTime);
    }
}