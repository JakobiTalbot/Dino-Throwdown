using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    // used to change movement for a limited time
    public struct PickupTimer
    {
        public bool bFlag;
        public float fTimer;
    }

    // used to distinguish between players
    public int m_cPlayerNumber = 1;
    // speed of the player
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
    // speed of the claw movement
    public float m_fClawSpeed = 15.0f;
    // speed at which the player is picked up
    public float m_fPickupSpeed = 15.0f;
    // reference to the arm which contains the weapon
    public GameObject m_arm;
    // reference to the crane seats
    public Transform[] m_seats;

    // reference to the claw that will be used
    [HideInInspector]
    public Claw m_claw;
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
    public PickupTimer m_cruiseControl;
    // handles when the player should have a larger weapon
    [HideInInspector]
    public PickupTimer m_weaponSize;

    private GamePadState m_gamePadState;
    private PlayerIndex m_playerIndex;

    private Rigidbody m_rigidbody;
    // contains weapon
    public GameObject m_weapon;
    // determines if the player is attacking
    private bool m_bIsAttacking = false;

    private Vector3 m_v3StartArmRotation;
    private Vector3 m_v3EndArmRotation;

    private Vector3 m_v3BaseWeaponScale;
    private Vector3 m_v3BaseWeaponPosition;

    private bool m_bRightTriggerDown;

    [HideInInspector]
    public bool m_bWeaponHit = false;

	// Use this for initialization
	void Awake()
    {
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
        m_weaponSize.bFlag = false;
        m_weaponSize.fTimer = 3.0f;

        // get start and end rotation for the weapon
        m_v3StartArmRotation = m_arm.transform.localRotation.eulerAngles;
        m_v3EndArmRotation = m_v3StartArmRotation;
        m_v3EndArmRotation.y -= 90.0f;

        m_v3BaseWeaponScale = m_weapon.transform.localScale;
        m_v3BaseWeaponPosition = m_weapon.transform.localPosition;
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

        // checks if the player is being picked up
        if (m_bPickedUp)
        {
            Pickup();
        }
        // checks if the player is in the crane
        else if (m_bInCrane && !m_bIsAttacking)
        {
            MoveClaw(v2Movement.x, v2Movement.y);
        }
        // checks if the player is on cruise control
        else if (m_cruiseControl.bFlag && !m_bIsOut && !m_bInCrane)
        {
            Cruise(v2Movement.x, v2Movement.y);
        }
        // checks if the player is in the game and not on cruise control
        else if (!m_bIsOut && !m_bInCrane)
        {
            Move(v2Movement.x, v2Movement.y);
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
        if ((Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f 
            || m_gamePadState.Triggers.Right > 0.0f) 
            && !m_bIsOut && !m_bRightTriggerDown)
        {
            // sets the player to attacking
            m_bIsAttacking = true;
            m_bRightTriggerDown = true;
        }

        if (m_gamePadState.Triggers.Right == 0.0f)
        {
            m_bRightTriggerDown = false;
        }

        // checks if the player is grabbing another with the claw
        if (m_bIsAttacking && m_bInCrane)
        {
            m_bIsAttacking = m_claw.Grab();
        }
        else if (m_bIsAttacking)
        {
            // swings the weapon
            Attack();
        }

        if (m_weaponSize.bFlag)
        {
            // decrements the timer
            m_weaponSize.fTimer -= Time.deltaTime;
            // checks if the timer has run out
            if (m_weaponSize.fTimer <= 0.0f)
            {
                // resets the weapon size
                m_weaponSize.bFlag = false;
                m_weaponSize.fTimer = 5.0f;
                m_weapon.transform.localScale = m_v3BaseWeaponScale;
                m_weapon.transform.localPosition = m_v3BaseWeaponPosition;
            }
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
        v3Direction.x = fHorizontal * m_fClawSpeed * Time.deltaTime;
        v3Direction.z = fVertical * m_fClawSpeed * Time.deltaTime;

        // moves the claw by the direction
        m_claw.transform.Translate(v3Direction);
    }

    // swings the weapon
    private void Attack()
    {
        // checks if the arm should still rotate
        if (m_arm.transform.localRotation.eulerAngles.y > (360.0f + m_v3EndArmRotation.y + 1.0f) || (m_arm.transform.localRotation.eulerAngles.y <= 0.1f && m_arm.transform.localRotation.eulerAngles.y >= -0.1f))
        {
            // rotates the arm
            m_arm.transform.localRotation = (Quaternion.Lerp(m_arm.transform.localRotation, Quaternion.Euler(m_v3EndArmRotation), m_fAttackSpeed));
        }
        else
        {
            StopAttack();
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
        if (transform.position.y < 14.0f)
        {
            // moves the player up
            transform.Translate(transform.up * Time.deltaTime * m_fPickupSpeed);
            // sets the object to kinematic
            m_rigidbody.isKinematic = true;
        }
        else
        {
            // creates a position underneath this object
            Vector3 v3Pos = new Vector3(transform.position.x, 3.1f, transform.position.z);
            // swaps the players
            m_claw.GetCrane().m_player.transform.position = v3Pos;
            m_claw.GetCrane().m_player.GetComponent<PlayerController>().m_bInCrane = false;
            m_claw.GetCrane().m_player.GetComponent<Rigidbody>().isKinematic = false;
            
            // resets the claw
            m_claw.transform.position = new Vector3(0.0f, 16.0f, 0.0f);
            Light light = m_claw.GetComponentInChildren<Light>();

            // sets the colour of the light to the colour of the player
            switch (m_cPlayerNumber)
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

            // sets this player as the one in the crane
            m_claw.GetCrane().m_player = gameObject;

            // sends the player to the crane
            if (m_claw.GetCrane().name == "Crane1")
            {
                transform.position = m_seats[0].transform.position;
            }
            else
            {
                transform.position = m_seats[1].transform.position;
            }

            transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

            // sets the status as not out or picked up and in the crane
            m_bIsOut = false;
            m_bInCrane = true;
            m_bPickedUp = false;
        }
    }

    public void StopAttack()
    {
        // sets the transform back to the original
        m_arm.transform.localRotation = Quaternion.Euler(m_v3StartArmRotation);

        // sets the player to not attacking
        m_bIsAttacking = false;
        m_bWeaponHit = false;
    }
}