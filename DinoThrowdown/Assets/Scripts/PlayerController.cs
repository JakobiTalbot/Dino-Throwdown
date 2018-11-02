using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
    // used to check for pick ups and time them
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
    // time to remain in cruise control
    public float m_fCruiseControlTime = 5.0f;
    // time to keep weapon size powerup
    public float m_fWeaponSizeTime = 3.0f;
    // contains weapon
    public GameObject m_weapon;
    // stores the amount of time to not be able to cruise after being hit
    public float m_fStopCruiseAfterHitTime = 2.0f;
    // reference to pause screen
    public PauseGame m_pauseGameCanvas;

    // reference to the claw that will be used
    [HideInInspector]
    public Claw m_claw;
    // handles when the player should cruise
    [HideInInspector]
    public PickupTimer m_cruiseControl;
    // handles when the player should have a larger weapon
    [HideInInspector]
    public PickupTimer m_weaponSize;
    // stores default weapon scale
    [HideInInspector]
    public Vector3 m_v3BaseWeaponScale;
    // stores default weapon position
    [HideInInspector]
    public Vector3 m_v3BaseWeaponPosition;
    // stores if the attack hit
    [HideInInspector]
    public bool m_bWeaponHit = false;
    // determines if the player is out of bounds
    [HideInInspector]
    public bool m_bIsOut = false;
    // determines if the player is in the crane
    [HideInInspector]
    public bool m_bInCrane = false;
    // stores timer for being knocked back
    [HideInInspector]
    public float m_fKnockedBackTimer = 0.0f;
    // a count of the amount of attacks the player has done while being picked up
    [HideInInspector]
    public int m_cAttackAmount = 0;

    private GamePadState m_gamePadState;
    private PlayerIndex m_playerIndex;
    private Rigidbody m_rigidbody;
    // determines if the player is attacking
    private Vector3 m_v3StartArmRotation;
    private Vector3 m_v3EndArmRotation;
    private float m_fVibrationTimer;
    private bool m_bIsAttacking = false;
    private bool m_bPauseButtonDown = false;
    private bool m_bVibrating = false;
    private bool m_bVibrationToggle = true;

    // Use this for initialization
    void Awake()
    {
        if (OptionsManager.InstanceExists)
        {
            m_bVibrationToggle = OptionsManager.Instance.m_bVibration;
        }

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
        m_cruiseControl.fTimer = m_fCruiseControlTime;
        m_weaponSize.bFlag = false;
        m_weaponSize.fTimer = m_fWeaponSizeTime;

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

        // checks if the player is in the crane
        if (m_bInCrane && !m_bIsAttacking && !m_bIsOut)
        {
            m_claw.Move(v2Movement.x, v2Movement.y, m_fClawSpeed);
        }
        // checks if the player is on cruise control
        else if (m_cruiseControl.bFlag && !m_bIsOut && !m_bInCrane)
        {
            if (m_fKnockedBackTimer <= 0.0f)
                Cruise(v2Movement.x, v2Movement.y);
            else
                Move(v2Movement.x, v2Movement.y);

            // look to movement
            if (v2Movement.sqrMagnitude != 0.0f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(v2Movement.x, 0.0f, v2Movement.y)), m_fRotateSpeedController);
        }
        // checks if the player is in the game and not on cruise control
        else if (!m_bIsOut && !m_bInCrane)
        {
            Move(v2Movement.x, v2Movement.y);

            // look to movement
            if (v2Movement.sqrMagnitude != 0.0f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(v2Movement.x, 0.0f, v2Movement.y)), m_fRotateSpeedController);
        }

        // pause game
        if (Input.GetButtonDown("Pause") || (m_gamePadState.Buttons.Start == ButtonState.Pressed && !m_bPauseButtonDown))
        {
            if (Time.timeScale > 0.0f)
            {
                Time.timeScale = 0.0f;
                m_pauseGameCanvas.gameObject.SetActive(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                m_pauseGameCanvas.ResetAlpha();
                m_pauseGameCanvas.gameObject.SetActive(false);
            }

            m_bPauseButtonDown = true;
        }
        else if (m_gamePadState.Buttons.Start == ButtonState.Released)
        {
            m_bPauseButtonDown = false;
        }

        // Activate dash (keyboard || controller)
        if ((Input.GetButtonDown("Jump" + m_cPlayerNumber.ToString()) || m_gamePadState.Triggers.Left > 0.0f) && !m_bIsOut && !m_bInCrane)
        {
            GetComponent<Dash>().DoDash();
        }

        if (!m_rigidbody.isKinematic && !m_bIsOut && !m_bInCrane)
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

        // checks if the player is in the crane and a button is pressed
        if (m_bInCrane && !m_bIsOut &&
            (m_gamePadState.Triggers.Right > 0.0f || m_gamePadState.Triggers.Left > 0.0f ||
             m_gamePadState.Buttons.RightShoulder == 0 || m_gamePadState.Buttons.LeftShoulder == 0 ||
             m_gamePadState.Buttons.A == 0 || m_gamePadState.Buttons.B == 0 ||
             m_gamePadState.Buttons.X == 0 || m_gamePadState.Buttons.Y == 0 ||
             Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f))
        {
            // sets the player to attacking
            m_bIsAttacking = true;
        }
        // checks if the fire button was pressed (keyboard || controller)
        else if (!m_bIsOut && (m_gamePadState.Triggers.Right > 0.0f || Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f) && !m_bIsAttacking)
        {
            // sets the player to attacking
            m_bIsAttacking = true;
            GetComponents<AudioSource>()[1].Play();
        }

        // checks if the player is grabbing another with the claw or dropping an item
        if (m_bIsAttacking && m_bInCrane && !m_bIsOut && !m_claw.m_delay.bFlag)
        {
            // checks if the claw has a player
            if (m_claw.m_bHasPlayer)
            {
                // attempts to drop the player
                m_bIsAttacking = m_claw.DropPlayer();
            }
            // checks if there is an item to drop
            else if (m_claw.m_bHasItem && !m_claw.m_bDropped)
            {
                m_claw.DropItem();
                m_bIsAttacking = false;
            }
            else
            {
                // grabs with the claw
                m_bIsAttacking = m_claw.Grab();
            }
        }
        else if (m_bIsAttacking && !m_bIsOut)
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
                m_weaponSize.fTimer = m_fWeaponSizeTime;
                m_weapon.transform.localScale = m_v3BaseWeaponScale;
                m_weapon.transform.localPosition = m_v3BaseWeaponPosition;
            }
        }

        if (m_cruiseControl.bFlag)
        {
            // decrements the timer
            m_cruiseControl.fTimer -= Time.deltaTime;
            // checks if the timer has run out
            if (m_cruiseControl.fTimer <= 0.0f)
            {
                // resets the cruise control
                m_cruiseControl.bFlag = false;
                m_cruiseControl.fTimer = m_fCruiseControlTime;
            }
        }

        if (m_bVibrating)
        {
            m_fVibrationTimer -= Time.deltaTime;

            if (m_fVibrationTimer <= 0.0f)
            {
                GamePad.SetVibration((PlayerIndex)m_cPlayerNumber - 1, 0.0f, 0.0f);
                m_bVibrating = false;
            }
        }

        m_fKnockedBackTimer -= Time.deltaTime;
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

        // Add velocity to rigidbody
        m_rigidbody.velocity += (v3Direction * m_fVelocity * Time.deltaTime);
    }

    // moves with little momentum based on input
    private void Cruise(float fHorizontal, float fVertical)
    {
        // direction based on input
        Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
        v3Direction.x = fHorizontal * m_fCruiseSpeed * Time.deltaTime;
        v3Direction.z = fVertical * m_fCruiseSpeed * Time.deltaTime;
        m_rigidbody.velocity = new Vector3(fHorizontal * m_fVelocity, m_rigidbody.velocity.y, fVertical * m_fVelocity);

        // moves the rigidbody by the direction
        transform.position += v3Direction;
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

    public void StopAttack()
    {
        // sets the transform back to the original
        m_arm.transform.localRotation = Quaternion.Euler(m_v3StartArmRotation);

        // sets the player to not attacking
        m_bIsAttacking = false;
        m_bWeaponHit = false;

        // increments the amount of attacks dealt if the player is picked up
        if (m_rigidbody.isKinematic)
        {
            m_cAttackAmount++;
        }
    }

    public void SetVibration(float fVibrationTime, float fVibrationStrength)
    {
        if (m_bVibrationToggle)
        {
            GamePad.SetVibration((PlayerIndex)m_cPlayerNumber - 1, fVibrationStrength, fVibrationStrength);
            m_fVibrationTimer = fVibrationTime;
            m_bVibrating = true;
        }
    }
}