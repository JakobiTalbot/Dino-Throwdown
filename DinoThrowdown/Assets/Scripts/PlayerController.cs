﻿using System.Collections;
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
    // the speed in which the body rotates when cruise control is enabled
    public float m_fCruiseRotateSpeed = 0.4f;
    // the movement speed during cruise control
    public float m_fCruiseSpeed = 8.0f;
    // speed of the claw movement
    public float m_fClawSpeed = 15.0f;
    // speed at which the player is picked up
    public float m_fPickupSpeed = 15.0f;
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
    // reference to the game over screen
    public RestartGame m_gameOverCanvas;
    // collection of references to the dinos
    public GameObject[] m_dinos;
    // reference to cruise control particle
    public GameObject m_cruiseControlParticle;
    // reference to smoke particle
    public ParticleSystem m_smokeParticle;
    // duration between input before buffer clears
    public float m_fKONAMIDuration = 2.0f;
    // delay between input
    public float m_fInputDelay = 0.2f;
    // reference to the konami camera
    public Camera m_konamiCamera;
    // rotation speed when konami is on
    public float m_fKonamiRotateSpeed = 5.0f;

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
    // determines if the player won
    [HideInInspector]
    public bool m_bWinner = false;
    // stores if the attack hit
    [HideInInspector]
    public bool m_bWeaponSwing = false;
    // stores if the attack hit
    [HideInInspector]
    public bool m_bWeaponHit = false;
    // determines if the movement should change due to the konami code
    [HideInInspector]
    public bool m_bKonami = false;

    private GamePadState m_gamePadState;
    private GameStateManager m_gameManager;
    private PlayerIndex m_playerIndex;
    private Rigidbody m_rigidbody;
    private float m_fVibrationTimer;
    private bool m_bIsAttacking = false;
    private bool m_bPauseButtonDown = false;
    private bool m_bVibrating = false;
    private bool m_bVibrationToggle = true;
    private bool m_bAttackButtonDown = false;
    // the player animator
    private Animator m_anim;
    // stores the player input as a string
    private string m_sBuffer = string.Empty;
    // the code that the buffer needs to match in order to utilise the konami code\
    private string m_sKonami = "uuddlrlrbas";
    // used to delay between input
    private Claw.PickupTimer m_inputDelay;
    // used to time input
    private float m_fKonamiTimer = 0.0f;
    private float m_fOriginalVolume = 1.0f;

    // Use this for initialization
    void Awake()
    {
        m_fOriginalVolume = m_weapon.GetComponent<AudioSource>().volume;

        if (OptionsManager.InstanceExists)
        {
            m_bVibrationToggle = OptionsManager.Instance.m_bVibration;
        }
        // get game manager script
        m_gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameStateManager>();
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
        m_inputDelay.bFlag = false;
        m_inputDelay.fTimer = m_fInputDelay;

        m_anim = GetComponentsInChildren<Animator>()[1];

        m_v3BaseWeaponScale = m_weapon.transform.localScale;
        m_v3BaseWeaponPosition = m_weapon.transform.localPosition;

        if (CharacterManager.InstanceExists)
        {
            m_dinos[m_cPlayerNumber - 1].SetActive(false);
            int iDinoType = CharacterManager.Instance.m_iDinoTypes[m_cPlayerNumber - 1];
            m_dinos[iDinoType].SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update()
    {
        if (CharacterManager.InstanceExists)
        {
            int iDinoType = CharacterManager.Instance.m_iDinoTypes[m_cPlayerNumber - 1];
            m_dinos[iDinoType].GetComponentInChildren<SkinnedMeshRenderer>().material = CharacterManager.Instance.m_dinoColours[m_cPlayerNumber - 1];
        }
        if (OptionsManager.InstanceExists)
        {
            m_weapon.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fMasterVolume * OptionsManager.Instance.m_fSFXVolume * m_fOriginalVolume;
        }

        if (!m_rigidbody.isKinematic)
        {
            m_cAttackAmount = 0;
        }

        // get controller input
        m_gamePadState = GamePad.GetState(m_playerIndex);
        // Get player input (keyboard)
        Vector2 v2Movement = new Vector2(Input.GetAxis("Horizontal" + m_cPlayerNumber.ToString()), Input.GetAxis("Vertical" + m_cPlayerNumber.ToString()));
        // Get player input (controller)
        v2Movement.x += m_gamePadState.ThumbSticks.Left.X;
        v2Movement.y += m_gamePadState.ThumbSticks.Left.Y;
        v2Movement.Normalize();

        // KONAMI
        KONAMI(m_gamePadState);

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
            if (v2Movement.sqrMagnitude != 0.0f && Time.timeScale > 0.0f && !m_bKonami)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(v2Movement.x, 0.0f, v2Movement.y)), m_fCruiseRotateSpeed);
            }
        }
        // checks if the player is in the game and not on cruise control
        else if (!m_bIsOut && !m_bInCrane)
        {
            Move(v2Movement.x, v2Movement.y);

            // look to movement
            if (v2Movement.sqrMagnitude != 0.0f && Time.timeScale > 0.0f && !m_bKonami)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(v2Movement.x, 0.0f, v2Movement.y)), m_fRotateSpeedController);
            }
        }

        // pause game
        if (!m_gameManager.m_bPlayerWon && (Input.GetButtonDown("Pause" + m_cPlayerNumber.ToString()) || (m_gamePadState.Buttons.Start == ButtonState.Pressed && !m_bPauseButtonDown)))
        {
            if (Time.timeScale > 0.0f)
            {
                Time.timeScale = 0.0f;
                m_pauseGameCanvas.gameObject.SetActive(true);
                m_pauseGameCanvas.m_buttons[0].Select();
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
        if (m_bInCrane && !m_bIsOut && !m_bAttackButtonDown &&
            (m_gamePadState.Triggers.Right > 0.0f || m_gamePadState.Triggers.Left > 0.0f ||
             m_gamePadState.Buttons.RightShoulder == 0 || m_gamePadState.Buttons.LeftShoulder == 0 ||
             m_gamePadState.Buttons.A == 0 || m_gamePadState.Buttons.B == 0 ||
             m_gamePadState.Buttons.X == 0 || m_gamePadState.Buttons.Y == 0 ||
             Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f))
        {
            // sets the player to attacking
            m_bIsAttacking = true;
        }
        // checks if the fire button was pressed
        else if (!m_bIsOut && !m_bAttackButtonDown && (m_gamePadState.Triggers.Right > 0.0f || Input.GetAxis("Fire" + m_cPlayerNumber.ToString()) > 0.0f) && !m_bIsAttacking)
        {
            // sets the player to attacking
            m_bIsAttacking = true;
        }

        if (m_gamePadState.Triggers.Right > 0.0f)
        {
            m_bAttackButtonDown = true;
        }
        else
        {
            m_bAttackButtonDown = false;
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
                m_cruiseControlParticle.SetActive(false);
                m_cruiseControl.bFlag = false;
                m_cruiseControl.fTimer = m_fCruiseControlTime;
                m_rigidbody.velocity = new Vector3(v2Movement.x * m_fVelocity, m_rigidbody.velocity.y, v2Movement.y * m_fVelocity);
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
        // moves relative to the direction the player is facing
        if (m_bKonami)
        {
            v3Direction += transform.forward * forwardMovement;
            if (rightMovement != 0.0f)
            {
                transform.Rotate(new Vector3(0.0f, rightMovement * m_fKonamiRotateSpeed, 0.0f));
            }
        }
        else
        {
            v3Direction += Vector3.forward * forwardMovement;
            v3Direction += Vector3.right * rightMovement;
        }

        if (v3Direction.sqrMagnitude > 1)
            v3Direction.Normalize();

        // Add velocity to rigidbody
        m_rigidbody.velocity += (v3Direction * m_fVelocity * Time.deltaTime);
    }

    // moves with little momentum based on input
    private void Cruise(float fHorizontal, float fVertical)
    {
        // direction based on input
        Vector3 v3Direction = new Vector3();
        // moves relative to the direction the player is facing
        if (m_bKonami)
        {
            v3Direction.x = transform.forward.x * fVertical * m_fCruiseSpeed * Time.deltaTime;
            v3Direction.z = transform.forward.z * fVertical * m_fCruiseSpeed * Time.deltaTime;
            if (fHorizontal != 0.0f)
            {
                transform.Rotate(new Vector3(0.0f, fHorizontal * m_fKonamiRotateSpeed, 0.0f));
            }
        }
        else
        {
            v3Direction.x = fHorizontal * m_fCruiseSpeed * Time.deltaTime;
            v3Direction.z = fVertical * m_fCruiseSpeed * Time.deltaTime;
        }

        // moves the rigidbody by the direction
        transform.position += v3Direction;
    }

    // swings the weapon
    private void Attack()
    {
        // starts attacking
        if (!m_bWeaponSwing && !m_weapon.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WeaponSwing"))
        {
            if (!m_anim.GetBool("bIsAttacking"))
            {
                m_anim.SetBool("bIsAttacking", true);
            }
            m_weapon.GetComponent<Animator>().SetTrigger("Attack");
            m_bWeaponSwing = true;
        }
    }

    public bool GetAttacking()
    {
        return m_bIsAttacking;
    }

    public void StopAttack()
    {
        // sets the player to not attacking
        m_bIsAttacking = false;
        m_bWeaponHit = false;

        // sets the animation back to normal
        if (m_weapon.GetComponent<Animator>() != null)
        {
            if (m_anim.GetBool("bIsAttacking"))
            {
                m_anim.SetBool("bIsAttacking", false);
            }
            m_bWeaponSwing = false;
        }

        // increments the amount of attacks dealt if the player is picked up
        if (m_rigidbody.isKinematic)
        {
            m_cAttackAmount++;
            GameObject[] claws = GameObject.FindGameObjectsWithTag("Claw");
            foreach (var claw in claws)
            {
                if (claw.GetComponent<Claw>().m_bHasPlayer)
                {
                    Instantiate(GetComponent<Knockback>().m_hitParticles, new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), transform.rotation);
                    break;
                }
            }
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

    public void KONAMI(GamePadState gamePadState)
    {
        // stores a letter if up, down, left, right, a, b or start is pressed and a dash if something else is pressed
        if (gamePadState.DPad.Up == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "u";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.DPad.Down == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "d";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.DPad.Left == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "l";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.DPad.Right == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "r";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.Buttons.B == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "b";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.Buttons.A == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "a";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.Buttons.Start == ButtonState.Pressed && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "s";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }
        else if (gamePadState.Buttons.X == ButtonState.Pressed && gamePadState.Buttons.Y == ButtonState.Pressed
                && gamePadState.Buttons.Back == ButtonState.Pressed && gamePadState.Buttons.Guide == ButtonState.Pressed
                && gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && gamePadState.Buttons.RightShoulder == ButtonState.Pressed
                && gamePadState.Buttons.LeftStick == ButtonState.Pressed && gamePadState.Buttons.RightStick == ButtonState.Pressed
                && gamePadState.Triggers.Left != 0.0f && gamePadState.Triggers.Right != 0.0f
                && gamePadState.ThumbSticks.Left.X != 0.0f && gamePadState.ThumbSticks.Left.Y != 0.0f
                && gamePadState.ThumbSticks.Right.X != 0.0f && gamePadState.ThumbSticks.Right.Y != 0.0f 
                && !m_inputDelay.bFlag)
        {
            if (m_fKonamiTimer >= m_fKONAMIDuration)
            {
                m_fKonamiTimer = 0.0f;
                m_sBuffer = string.Empty;
            }

            m_sBuffer += "-";
            m_inputDelay.bFlag = true;
            m_fKonamiTimer = 0.0f;
        }

        // checks if too many characters are given
        if (m_sBuffer.Length > 11)
        {
            // removes the first character
            char[] sBuffer = m_sBuffer.ToCharArray(1, 11);
            m_sBuffer = string.Empty;
            for (int i = 0; i < sBuffer.Length; i++)
            {
                m_sBuffer += sBuffer[i];
            }
        }

        // ensures the user is not taking too long to input the code
        if (m_fKonamiTimer <= m_fKONAMIDuration)
        {
            m_fKonamiTimer += Time.deltaTime;
        }

        // checks if the konami code was entered
        if (m_sBuffer == m_sKonami && !m_bKonami)
        {
            m_gameOverCanvas.KONAMION();
            m_sBuffer = string.Empty;
        }
        else if (m_sBuffer == m_sKonami && m_bKonami)
        {
            m_gameOverCanvas.KONAMIOFF();
            m_sBuffer = string.Empty;
        }

        if (m_inputDelay.bFlag)
        {
            m_inputDelay.fTimer -= Time.deltaTime;
            if (m_inputDelay.fTimer < 0.0f)
            {
                m_inputDelay.fTimer = m_fInputDelay;
                m_inputDelay.bFlag = false;
            }
        }
    }

    // turns on the konami function
    public void KonamiOn()
    {
        m_bKonami = true;
        m_konamiCamera.gameObject.SetActive(true);
    }
    // turns off the konami function
    public void KonamiOff()
    {
        m_bKonami = false;
        m_konamiCamera.gameObject.SetActive(false);
    }
}