using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    // used to change movement for a limited time
    public struct CruiseControl
    {
        public bool bFlag;
        public float fTimer;
    }

    // used to distinguish between players
    public int m_cPlayerNumber = 1;
    public float m_fVelocity = 10.0f;
    // how fast the object lerps rotation to X and Z = 0
    public float m_fCorrectionSpeed = 0.01f;
    // the height at which the player hovers
    public float m_fHoverHeight = 2.0f;
    // the upwards force to push the rigidbody when above another object
    public float m_fHoverForce = 550.0f;
    // the effectiveness of height decreasing hover force
    public float m_fHeightFromHoverMultiplier = 50.0f;
    // the speed in which the body rotates for keyboard input
    public float m_fRotateSpeedKeyboard = 10.0f;
    // the speed in which the body rotates for controller input
    public float m_fRotateSpeedController = 0.1f;
    // the movement speed during cruise control
    public float m_fCruiseSpeed = 8.0f;
    // the speed at which the weapon swings
    public float m_fAttackSpeed = 0.1f;
    // reference to the arm which contains the weapon
    public GameObject m_arm;
    // reference to the crane
    public CraneOccupied m_crane;

    // determines if the player is being picked up
    [HideInInspector]
    public bool m_bPickedUp = false;
    // determines if the player is out of bounds
    [HideInInspector]
    public bool m_bIsOut = false;
    // determines if the player is in the crane
    [HideInInspector]
    public bool m_bInCrane = false;
    // handles when the player should cruise
    [HideInInspector]
    public CruiseControl m_cruiseControl;
    // reference to the claw that will be used
    [HideInInspector]
    public GameObject m_claw;

    private GamePadState m_gamePadState;
    private PlayerIndex m_playerIndex;

    private Rigidbody m_rigidbody;
    // contains weapon
    public GameObject m_weapon;
    // determines if the player is attacking
    private bool m_bIsAttacking = false;
    // determines if the player is grabbing with the claw
    private bool m_bIsGrabbing = false;

    private Vector3 v3StartArmRotation;
    private Vector3 v3EndArmRotation;

	// Use this for initialization
	void Awake()
    {
        // Ignore collision between weapon and player
        Physics.IgnoreCollision(m_arm.GetComponentInChildren<BoxCollider>(), GetComponent<MeshCollider>());
        m_rigidbody = GetComponent<Rigidbody>();

        // Get playerindex (-1 because XInput starts index at 0)
        m_playerIndex = (PlayerIndex)m_cPlayerNumber - 1;

        // get player's gamepad
        m_gamePadState = GamePad.GetState(m_playerIndex);

        // Set friction to 0 so object doesn't get stuck to other objects
        GetComponent<MeshCollider>().material = new PhysicMaterial()
        {
            dynamicFriction = 0,
            frictionCombine = PhysicMaterialCombine.Minimum
        };

        m_cruiseControl.bFlag = false;
        m_cruiseControl.fTimer = 5.0f;

        v3StartArmRotation = m_arm.transform.localRotation.eulerAngles;
        v3EndArmRotation = v3StartArmRotation;
        v3EndArmRotation.y -= 90.0f;
    }
	
	// Update is called once per frame
	void Update()
    {
        // get controller input
        m_gamePadState = GamePad.GetState(m_playerIndex);
        // Get player input (keyboard)
        Vector2 v2Movement = new Vector2(Input.GetAxis("Horizontal" + m_cPlayerNumber.ToString()), Input.GetAxis("Vertical" + m_cPlayerNumber.ToString()));
        // Get player input (controller)
        v2Movement.x += m_gamePadState.ThumbSticks.Left.X;
        v2Movement.y += m_gamePadState.ThumbSticks.Left.Y;

        v2Movement.Normalize();

        // checks if the player is in the game and not on cruise control
        if (!m_cruiseControl.bFlag && !m_bIsOut && !m_bInCrane)
        {
            Move(v2Movement.x, v2Movement.y);
        }
        // checks if the player is on cruise control
        else if (!m_bIsOut && !m_bInCrane)
        {
            Cruise(v2Movement.x, v2Movement.y);
        }
        // checks if the player is in the crane
        else if (!m_bIsOut)
        {
            MoveClaw(v2Movement.x, v2Movement.y);
            // checks if the player is grabbing another with the claw
            if (m_bIsGrabbing)
            {
                m_bIsGrabbing = m_claw.GetComponent<Claw>().Grab();
            }
        }
        // checks if the player is being picked up
        else if (m_bPickedUp)
        {
            Pickup();
        }

        // get gamepad right stick input
        float rightRotation = m_gamePadState.ThumbSticks.Right.X;
        float forwardRotation = m_gamePadState.ThumbSticks.Right.Y;

        // rotate (controller)
        Vector3 v3LookDirection = new Vector3(rightRotation, 0.0f, forwardRotation);
        if (v3LookDirection.magnitude > 0.0f && !m_bIsOut && !m_bInCrane)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(v3LookDirection), m_fRotateSpeedController);
        }
        // rotate (keyboard)
        if (Input.GetButton("Rotate" + m_cPlayerNumber.ToString()) && !m_bIsOut && !m_bInCrane)
        {
            float fRotate = Input.GetAxis("Rotate" + m_cPlayerNumber.ToString());
            transform.Rotate(new Vector3(0.0f, fRotate * m_fRotateSpeedKeyboard, 0.0f));
        }

        // Activate dash (keyboard || controller)
        if ((Input.GetButtonDown("Jump" + m_cPlayerNumber.ToString()) || m_gamePadState.Triggers.Left > 0.0f) && !m_bIsOut && !m_bInCrane)
        {
            GetComponent<Dash>().DoDash();
        }

        if (!m_bPickedUp && !m_bIsOut && !m_bInCrane)
        {
            RaycastHit ray = new RaycastHit();

            // Get origin of raycast above player so it doesn't bug when just using player position
            Vector3 v3OriginPos = transform.position;
            v3OriginPos.y += 1.0f;
            // raycast to hit ground
            if (Physics.Raycast(v3OriginPos, Vector3.down, out ray, m_fHoverHeight)
                && ray.collider.CompareTag("Ground"))
            {
                m_rigidbody.AddForce(Vector3.up * m_rigidbody.mass * (m_fHoverForce - (transform.position.y * m_fHeightFromHoverMultiplier)) * Time.deltaTime);
            }
        }

        // Nullify angular velocity so it doesn't conflict with quaternion lerp
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);

        //Correct X and Z rotation of object
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f), m_fCorrectionSpeed);

        // checks if the fire button was pressed (keyboard || controller)
        if ((Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f || m_gamePadState.Triggers.Right > 0.0f) && !m_bIsOut && !m_bInCrane)
        {
            // sets the player to attacking
            m_bIsAttacking = true;
        }
        else if ((Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f || m_gamePadState.Triggers.Right > 0.0f) && !m_bIsOut)
        {
            // sets the player to grabbing
            m_bIsGrabbing = true;
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
        // prevent stutter
        m_rigidbody.velocity = Vector3.zero;
        // direction based on input
        Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
        v3Direction.x = fHorizontal * m_fCruiseSpeed * Time.deltaTime;
        v3Direction.z = fVertical * m_fCruiseSpeed * Time.deltaTime;

        // moves the rigidbody by the direction
        transform.position += v3Direction;

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

    private void MoveClaw(float fHorizontal, float fVertical)
    {
        //direction based on input
        Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
        v3Direction.x = fHorizontal * m_fCruiseSpeed * Time.deltaTime;
        v3Direction.z = fVertical * m_fCruiseSpeed * Time.deltaTime;

        // moves the claw by the direction
        m_claw.GetComponent<Rigidbody>().MovePosition(m_claw.transform.position + v3Direction);
    }

    // swings the weapon
    private void Attack()
    {
        // checks if the arm should still rotate
        if (m_arm.transform.localRotation.eulerAngles.y > (360.0f + v3EndArmRotation.y + 1.0f) || (m_arm.transform.localRotation.eulerAngles.y <= 0.1f && m_arm.transform.localRotation.eulerAngles.y >= -0.1f))
        {
            // rotates the arm
            m_arm.transform.localRotation = (Quaternion.Lerp(m_arm.transform.localRotation, Quaternion.Euler(v3EndArmRotation), m_fAttackSpeed));
        }
        else
        {
            // sets the transform back to the original
            m_arm.transform.localRotation = Quaternion.Euler(v3StartArmRotation);
            m_weapon.transform.localScale = new Vector3(2.0f, 0.2f, 0.2f);
            // sets the player to not attacking
            m_bIsAttacking = false;
        }
    }

    public bool GetAttacking()
    {
        return m_bIsAttacking;
    }

    // picks up the player and sends them to the crane
    public void Pickup()
    {
        // checks if the player is below the desired height
        if (transform.position.y < 13.0f)
        {
            // moves the player up
            transform.Translate(transform.up * Time.deltaTime * 10.0f);
            // sets the object to kinematic
            m_rigidbody.isKinematic = true;
        }
        else
        {
            // checks if there is already a player at the crane
            if (m_crane.m_bOccupied)
            {
                // creates a position underneath this object
                Vector3 v3Pos = new Vector3(transform.position.x, 3.1f, transform.position.z);
                // swaps the players
                m_crane.m_player.transform.position = v3Pos;
                m_crane.m_player.GetComponent<PlayerController>().m_bInCrane = false;
                m_crane.m_player.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                // sets the crane to occupied
                m_crane.m_bOccupied = true;
            }

            // resets the claw
            m_claw.transform.position = new Vector3(0.0f, 15.0f, 0.0f);

            // sets the colour of the light to the colour of the player
            switch (m_cPlayerNumber)
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

            // sets this player as the one in the crane
            m_crane.m_player = gameObject;

            // sends the player to the crane
            transform.position = new Vector3(-22.5f, 7.3f, 31.5f);
            transform.localRotation = Quaternion.Euler(0.0f, 144.0f, 0.0f);
            // sets the status as not out or picked up and in the crane
            m_bIsOut = false;
            m_bInCrane = true;
            m_bPickedUp = false;
        }
    }
}