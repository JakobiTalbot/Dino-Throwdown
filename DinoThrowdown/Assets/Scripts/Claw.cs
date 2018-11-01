using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    // used to check for pick ups and time them
    public struct PickupTimer
    {
        public bool bFlag;
        public float fTimer;
    }

    // speed at which the claw drops
    public float m_fMoveSpeed = 10.0f;
    // speed at which items fall
    public float m_fFallSpeed = 15.0f;
    // distance the claw can move from the origin
    public float m_fMoveRadius = 30.0f;
    // types of items to pick up
    public GameObject[] m_itemTypes;
    // the length of the line
    public float m_fLineLength = 13.5f;
    // the amount of attacks required to drop a player
    public int m_cClawHealth = 3;
    // time to remain in cruise control
    public float m_fCruiseControlTime = 5.0f;
    // time to keep weapon size powerup
    public float m_fWeaponSizeTime = 3.0f;
    // time to keep knockback shield pick up
    public float m_fKnockbackShieldTime = 5.0f;
    // delay time after a player breaks free
    public float m_fDelayTime = 1.0f;
    // the speed of the claw when cruising
    public float m_fCruiseSpeed = 25.0f;
    // the size multiplier when the claw is bigger
    public float m_fWeaponSizeMultiplier = 2.0f;
    // the health of the claw with the shield
    public int m_cShieldHealth = 6;

    // determines if the claw has dropped
    [HideInInspector]
    public bool m_bDropped = false;
    // determines if the claw picked up an item
    [HideInInspector]
    public bool m_bHasItem = false;
    // determines if the item is being dropped
    [HideInInspector]
    public bool m_bItemDrop = false;
    // reference to the item that is picked up
    [HideInInspector]
    public GameObject m_item = null;
    // determines if the claw has a player
    [HideInInspector]
    public bool m_bHasPlayer = false;
    // handles when the claw should cruise
    [HideInInspector]
    public PickupTimer m_cruiseControl;
    // handles when the claw should be larger
    [HideInInspector]
    public PickupTimer m_weaponSize;
    // handles when the claw should have a knockback shield
    [HideInInspector]
    public PickupTimer m_knockbackShield;
    // delays the claw after having a picked up player break free
    [HideInInspector]
    public PickupTimer m_delay;
    // reference to the player being picked up
    [HideInInspector]
    public PlayerController m_pickedUpPlayer = null;
    // used to stop the claw from activating the inner ring script on the first frame
    [HideInInspector]
    public bool m_bFirstFrame = true;

    // reference to the crane
    private CraneManager m_crane;
    //private bool m_bClawDownSoundPlayed = false;
    //private bool m_bClawUpSoundPlayed = false;

    private void Start()
    {
        m_crane = GetComponentInParent<CraneManager>();

        // sets the claw to have no pick ups
        m_cruiseControl.bFlag = false;
        m_cruiseControl.fTimer = m_fCruiseControlTime;
        m_weaponSize.bFlag = false;
        m_weaponSize.fTimer = m_fWeaponSizeTime;
        m_knockbackShield.bFlag = false;
        m_knockbackShield.fTimer = m_fKnockbackShieldTime;
        m_delay.bFlag = false;
        m_delay.fTimer = m_fDelayTime;
    }

    private void Update()
    {
        // checks if there is someone in the crane
        if (m_crane.m_bOccupied)
        {
            LineRenderer line = GetComponentInChildren<LineRenderer>();
            line.SetPosition(1, new Vector3(0.0f, 0.0f, m_fLineLength));
            // sets the colour of the line to the colour of the player
            line.endColor = m_crane.m_player.GetComponent<MeshRenderer>().material.color;

            // checks if the claw has a player and a shield
            if (m_bHasPlayer && m_knockbackShield.bFlag)
            {
                // checks if the amount of time the player has hit is equal to the health
                if (m_pickedUpPlayer.m_cAttackAmount == m_cShieldHealth)
                {
                    // lets go of the player
                    m_pickedUpPlayer.m_cAttackAmount = 0;
                    m_pickedUpPlayer.GetComponent<Rigidbody>().isKinematic = false;
                    m_bHasPlayer = false;
                    transform.position = new Vector3(transform.position.x, 16.0f, transform.position.z);
                    m_delay.bFlag = true;
                }
            }
            // checks if the claw has a player
            else if (m_bHasPlayer)
            {
                // checks if the amount of time the player has hit is equal to the health
                if (m_pickedUpPlayer.m_cAttackAmount == m_cClawHealth)
                {
                    // lets go of the player
                    m_pickedUpPlayer.m_cAttackAmount = 0;
                    m_pickedUpPlayer.GetComponent<Rigidbody>().isKinematic = false;
                    m_bHasPlayer = false;
                    transform.position = new Vector3(transform.position.x, 16.0f, transform.position.z);
                    m_delay.bFlag = true;
                }
            }
        }
        else
        {
            // sets the line length to zero
            GetComponentInChildren<LineRenderer>().SetPosition(1, Vector3.zero);
        }

        if (m_delay.bFlag)
        {
            // decrements the timer
            m_delay.fTimer -= Time.deltaTime;
            // checks if the timer has run out
            if (m_delay.fTimer <= 0.0f)
            {
                // resets the delay
                m_delay.bFlag = false;
                m_delay.fTimer = m_fDelayTime;
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
        else if (m_weaponSize.bFlag)
        {
            // decrements the timer
            m_weaponSize.fTimer -= Time.deltaTime;
            // checks if the timer has run out
            if (m_weaponSize.fTimer <= 0.0f)
            {
                // resets the weapon size
                m_weaponSize.bFlag = false;
                m_weaponSize.fTimer = m_fWeaponSizeTime;
                transform.localScale /= m_fWeaponSizeMultiplier;
                // resets the line size
                GetComponentInChildren<LineRenderer>().widthMultiplier /= m_fWeaponSizeMultiplier;
                m_fLineLength *= m_fWeaponSizeMultiplier;
            }
        }
        else if (m_knockbackShield.bFlag)
        {
            // decrements the timer
            m_knockbackShield.fTimer -= Time.deltaTime;
            // checks if the timer has run out
            if (m_knockbackShield.fTimer <= 0.0f)
            {
                // resets the knockback shield
                m_knockbackShield.bFlag = false;
                m_knockbackShield.fTimer = m_fKnockbackShieldTime;
            }
        }

    }

    // returns the crane
    public CraneManager GetCrane()
    {
        return m_crane;
    }

    // drops the claw
    private void Drop()
    {
        // checks if the claw is above the target height
        if (transform.localPosition.y > -10.0f)
        {
            // moves the claw down
            transform.Translate(-transform.up * Time.deltaTime * m_fMoveSpeed);
            //if (!m_bClawDownSoundPlayed)
            //    GetComponents<AudioSource>()[0].Play();
            //m_bClawDownSoundPlayed = true;
        }
        else
        {
            // creates an item at the claw
            m_item = Instantiate(m_itemTypes[0], transform);
            m_item.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            m_bHasItem = true;
            // sets the dropped status to true
            m_bDropped = true;
        }
    }
    // raises the claw
    private bool Raise()
    {
        // checks if the claw is below the target height
        if (transform.localPosition.y < 10.0f && !m_bHasPlayer)
        {
            // moves the claw up
            transform.Translate(transform.up * Time.deltaTime * m_fMoveSpeed);
            //if (!m_bClawUpSoundPlayed)
            //    GetComponents<AudioSource>()[1].Play();
            //m_bClawUpSoundPlayed = true;
            return true;
        }
        // checks if the claw has a player
        else if (transform.localPosition.y < 3.0f && m_bHasPlayer)
        {
            // moves the claw up
            transform.Translate(transform.up * Time.deltaTime * m_fMoveSpeed);
            m_pickedUpPlayer.transform.Translate(transform.up * Time.deltaTime * m_fMoveSpeed);
            m_pickedUpPlayer.transform.position = Vector3.Lerp(m_pickedUpPlayer.transform.position,
                new Vector3(transform.position.x, m_pickedUpPlayer.transform.position.y, transform.position.z),
                m_fMoveSpeed * Time.deltaTime);
            return true;
        }
        else
        {
            // the claw has risen back to the top
            m_bDropped = false;
            //m_bClawDownSoundPlayed = false;
            //m_bClawUpSoundPlayed = false;
            return false;
        }
    }
    // drops then rises
    public bool Grab()
    {
        // checks if the claw has already dropped
        if (!m_bDropped)
        {
            Drop();
        }
        else
        {
            return Raise();
        }

        return true;
    }

    // moves the claw and potentially the item being held
    public void Move(float fHorizontal, float fVertical, float fSpeed)
    {
        //direction based on input
        Vector3 v3Direction = new Vector3(0.0f, 0.0f, 0.0f);
        v3Direction.x = fHorizontal * Time.deltaTime;
        v3Direction.z = fVertical * Time.deltaTime;

        // stores the previous item position
        Vector3 v3PrevPos = Vector3.zero;
        if (m_bHasItem && m_bItemDrop)
        {
            v3PrevPos = m_item.transform.position;
        }

        // changes the moving speed based on the pick up
        if (m_cruiseControl.bFlag)
        {
            // moves the claw by the direction
            transform.Translate(v3Direction * m_fCruiseSpeed);
        }
        else
        {
            // moves the claw by the direction
            transform.Translate(v3Direction * fSpeed);
        }
        
        // moves the picked up player to the claw position
        if (m_bHasPlayer)
        {
            m_pickedUpPlayer.transform.position = new Vector3(transform.position.x, m_pickedUpPlayer.transform.position.y, transform.position.z);
        }

        // gets the claw's position in the x and z axis
        Vector2 v2Origin = new Vector2(transform.position.x, transform.position.z);
        // checks if the claw is too far
        if (v2Origin.magnitude > m_fMoveRadius)
        {
            // sets the claw's position at the limit
            v2Origin.Normalize();
            v2Origin *= m_fMoveRadius;
            transform.position = new Vector3(v2Origin.x, transform.position.y, v2Origin.y);
        }

        if (m_bItemDrop)
        {
            // ensures that the item falls straight down
            m_item.transform.position = new Vector3(v3PrevPos.x, m_item.transform.position.y, v3PrevPos.z);
            m_item.transform.Translate(-m_item.transform.up * Time.deltaTime * m_fFallSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if the claw collides with a player
        if (other.CompareTag("Player") && !other.GetComponent<PlayerController>().m_bInCrane && !other.GetComponent<Rigidbody>().isKinematic && !m_bHasPlayer)
        {
            // sets the claw to have a player
            m_bHasPlayer = true;
            m_pickedUpPlayer = other.GetComponent<PlayerController>();
            m_pickedUpPlayer.GetComponent<Rigidbody>().isKinematic = true;
            // sets the claw to have dropped
            m_bDropped = true;

            if (OptionsManager.InstanceExists)
            {
                other.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
            }

            // plays the audio
            other.GetComponent<AudioSource>().Play();
        }
        // checks if the claw collided with the platform
        else if (other.CompareTag("Ground"))
        {
            // sets the claw to have dropped
            m_bDropped = true;
        }
    }

    // sets the object to not kinematic so that it drops
    public void DropItem()
    {
        if (m_item != null && !m_bDropped)
        {
            m_item.GetComponent<Rigidbody>().isKinematic = false;
            m_bItemDrop = true;
        }
    }

    // attempts to drop the player outside the arena
    public bool DropPlayer()
    {
        // checks if the claw has already dropped
        if (!m_bDropped)
        {
            LowerPlayer();
        }
        else
        {
            return Raise();
        }

        return true;
    }
    // moves the player down and swaps the players
    public void LowerPlayer()
    {
        // checks if the claw is above the target height
        if (transform.localPosition.y > -10.0f)
        {
            // moves the claw down
            transform.Translate(-transform.up * Time.deltaTime * m_fMoveSpeed);
            m_pickedUpPlayer.transform.Translate(-transform.up * Time.deltaTime * m_fMoveSpeed);
        }
        else
        {
            // sets the dropped status to true
            m_bDropped = true;

            // moves the player from the crane back into the arena
            m_crane.m_player.transform.position = new Vector3(0.0f, 5.0f, 0.0f);
            m_crane.m_player.GetComponent<PlayerController>().m_bInCrane = false;
            m_crane.m_player.GetComponent<PlayerController>().m_claw = null;
            m_crane.m_player.GetComponent<Rigidbody>().isKinematic = false;

            // resets the claw
            transform.position = new Vector3(0.0f, 16.0f, 0.0f);
            m_bHasPlayer = false;

            // sets the picked up player as the one in the crane
            m_crane.m_player = m_pickedUpPlayer.gameObject;

            // sends the player to the crane
            if (m_crane.name == "Crane1")
            {
                m_pickedUpPlayer.transform.position = m_pickedUpPlayer.m_seats[0].transform.position;
                m_pickedUpPlayer.transform.localRotation = Quaternion.Euler(0.0f, m_pickedUpPlayer.m_seats[0].transform.rotation.eulerAngles.y, 0.0f);
            }
            else
            {
                m_pickedUpPlayer.transform.position = m_pickedUpPlayer.m_seats[1].transform.position;
                m_pickedUpPlayer.transform.localRotation = Quaternion.Euler(0.0f, m_pickedUpPlayer.m_seats[1].transform.rotation.eulerAngles.y, 0.0f);
            }

            // sets the status as in the crane
            m_pickedUpPlayer.m_bIsOut = false;
            m_pickedUpPlayer.m_bInCrane = true;
            m_pickedUpPlayer.m_claw = this;
            m_pickedUpPlayer.m_cAttackAmount = 0;
        }
    }
}