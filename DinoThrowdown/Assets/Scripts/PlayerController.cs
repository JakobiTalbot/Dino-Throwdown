using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_fForce = 1000.0f;
    public float m_fCorrectionSpeed = 0.01f;

    private Rigidbody m_rigidbody;

	// Use this for initialization
	void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        // Set friction to 0 so object doesn't get stuck to other objects
        GetComponent<BoxCollider>().material = new PhysicMaterial()
        {
            dynamicFriction = 0,
            frictionCombine = PhysicMaterialCombine.Minimum
        };
	}
	
	// Update is called once per frame
	void Update()
    {
        // Get player input
        float rightMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");

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