using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // used to change movement for a limited time
    public struct CruiseControl
    {
        public bool bFlag;
        public float fTimer;
    }

    public float m_fVelocity = 10.0f;
    // how fast the object lerps rotation to X and Z = 0
    public float m_fCorrectionSpeed = 0.01f;
    // the upwards force to push the rigidbody when above another object
    public float m_fHoverForce = 550.0f;
    // the speed in which the body rotates
    public float m_fRotateSpeed = 10.0f;
    // the movement speed during cruise control
    public float m_fCruiseSpeed = 8.0f;
    // the speed at which the weapon swings
    public float m_fAttackSpeed = 0.1f;
    // reference to the arm which contains the weapon
    public GameObject m_arm;
    // handles when the player should cruise
    [HideInInspector]
    public CruiseControl m_cruiseControl;

    private Rigidbody m_rigidbody;
    // reference to the rigidbody of the arm
    private Rigidbody m_armRigidbody;
    // determines if the player is attacking
    private bool m_bIsAttacking = false;

	// Use this for initialization
	void Awake()
    {
        // Ignore collision between weapon and player
        Physics.IgnoreCollision(m_arm.GetComponentInChildren<BoxCollider>(), GetComponent<BoxCollider>());
        m_rigidbody = GetComponent<Rigidbody>();
        m_armRigidbody = m_arm.GetComponent<Rigidbody>();

        // Set friction to 0 so object doesn't get stuck to other objects
        GetComponent<BoxCollider>().material = new PhysicMaterial()
        {
            dynamicFriction = 0,
            frictionCombine = PhysicMaterialCombine.Minimum
        };

        m_cruiseControl.bFlag = false;
        m_cruiseControl.fTimer = 5.0f;
	}
	
	// Update is called once per frame
	void Update()
    {
        // Get player input
        float rightMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");

        // moves based on the cruise control flag
        if (!m_cruiseControl.bFlag)
        {
            Move(rightMovement, forwardMovement);
        }
        else
        {
            Cruise(rightMovement, forwardMovement);
        }

        if (Input.GetButton("Rotate"))
        {
            float fRotate = Input.GetAxis("Rotate");
            transform.Rotate(new Vector3(0.0f, fRotate * m_fRotateSpeed, 0.0f));
        }

        // Activate dash
        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Dash>().DoDash();
        }

        // Get origin of raycast above player so it doesn't bug when just using player position
        Vector3 v3OriginPos = transform.position;
        v3OriginPos.y += 1.0f;
        if (Physics.Raycast(v3OriginPos, Vector3.down, 2.5f))
        {
            m_rigidbody.AddForce(Vector3.up * m_rigidbody.mass * (550.0f - transform.position.y) * Time.deltaTime);
        }

        // Nullify angular velocity so it doesn't conflict with quaternion lerp
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);

        //Correct X and Z rotation of object
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f), m_fCorrectionSpeed);

        // checks if the fire button was pressed
        if (Input.GetAxisRaw("Fire1") != 0)
        {
            // sets the player to attacking
            m_bIsAttacking = true;
        }

        if (m_bIsAttacking)
        {
            // swings the weapon
            Attack();
        }
    }

    // moves normally based on input
    private void Move(float rightMovement, float forwardMovement)
    {
        // Put input into force vector3
        Vector3 v3Direction = new Vector3();
        v3Direction += Vector3.forward * forwardMovement;
        v3Direction += Vector3.right * rightMovement;

        if (v3Direction.sqrMagnitude > 1)
            v3Direction.Normalize();

        // Add force to rigidbody
        m_rigidbody.velocity += (v3Direction * m_fVelocity * Time.deltaTime);
    }

    // moves with little momentum based on input
    private void Cruise(float fHorizontal, float fVertical)
    {
        //direction based on input
        Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
        v3Direction.x = fHorizontal * m_fCruiseSpeed * Time.deltaTime;
        v3Direction.z = fVertical * m_fCruiseSpeed * Time.deltaTime;

        // moves the rigidbody by the direction
        m_rigidbody.MovePosition(m_rigidbody.position + v3Direction);

        // decrements the timer
        m_cruiseControl.fTimer -= Time.deltaTime;
        // checks if the timer has run out
        if (m_cruiseControl.fTimer <= 0.0f)
        {
            // resets the cruise control
            m_cruiseControl.bFlag = false;
            m_cruiseControl.fTimer = 5.0f;
        }
    }

    // swings the weapon
    private void Attack()
    {
        // checks if the arm should still rotate
        if (m_arm.transform.localRotation.eulerAngles.y > 271.0f || (m_arm.transform.localRotation.eulerAngles.y <= 0.1f && m_arm.transform.localRotation.eulerAngles.y >= -0.1f))
        {
            // rotates the arm
            m_arm.transform.localRotation = (Quaternion.Lerp(m_arm.transform.localRotation, Quaternion.Euler(0.0f, -90, 0.0f), m_fAttackSpeed));
            // ensures that the arm is in the same position
            m_arm.transform.localPosition = new Vector3(0.75f, 0.75f, 0.5f);
        }
        else
        {
            // sets the transform back to the original
            m_arm.transform.localScale = new Vector3(2.0f, 0.2f, 0.2f);
            m_arm.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            m_arm.transform.localPosition = new Vector3(0.75f, 0.75f, 0.5f);
            // sets the player to not attacking
            m_bIsAttacking = false;
        }
    }
}