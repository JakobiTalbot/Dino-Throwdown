using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public float m_fVelocity = 10.0f;
    public float m_fCorrectionSpeed = 0.01f;

    private GameObject[] m_enemyObjects;
    private GameObject m_targetObject;
    private Rigidbody m_rigidbody;

    // Use this for initialization
    void Awake()
    {
        m_enemyObjects = GameObject.FindGameObjectsWithTag("Player");
        m_targetObject = m_enemyObjects[0];
        m_rigidbody = GetComponent<Rigidbody>();

        // Set friction to 0 so object doesn't get stuck to other objects
        GetComponent<MeshCollider>().material = new PhysicMaterial()
        {
            dynamicFriction = 0,
            frictionCombine = PhysicMaterialCombine.Minimum
        };
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

        // Get origin of raycast above player so it doesn't bug when just using player position
        Vector3 v3OriginPos = transform.position;
        v3OriginPos.y += 1.0f;
        if (Physics.Raycast(v3OriginPos, Vector3.down, 2.5f))
        {
            m_rigidbody.AddForce(Vector3.up * m_rigidbody.mass * (550.0f - transform.position.y) * Time.deltaTime);
        }

        // Nullify angular velocity
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);

        //Correct X and Z rotation of object
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f), m_fCorrectionSpeed);

        // Get direction to enemy
        Vector3 v3Dir = m_targetObject.transform.position - transform.position;

        v3Dir.y = 0.0f; // Nullify y force so agent doesn't fly when falling off edge

        if (v3Dir.sqrMagnitude > 1)
            v3Dir.Normalize();

        // Add force to rigidbody
        m_rigidbody.velocity += (v3Dir * m_fVelocity * Time.deltaTime);
    }
}