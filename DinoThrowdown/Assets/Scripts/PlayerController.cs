using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_fForce = 1000.0f;

    private Rigidbody m_rigidbody;

	// Use this for initialization
	void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update()
    {
        // Get player input
        float rightMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");

        // Put input into force vector3
        Vector3 v3Force = new Vector3();
        v3Force += Vector3.forward * forwardMovement;
        v3Force += Vector3.right * rightMovement;

        if (v3Force.sqrMagnitude > 1)
            v3Force.Normalize();

        // Add force to rigidbody
        m_rigidbody.AddForce(v3Force * m_fForce * Time.deltaTime);
    }
}