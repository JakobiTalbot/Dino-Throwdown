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

    // reference to bombdropper
    public GameObject m_bombDropper;
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
    // delay time after a bomb is dropped
    public float m_fBombDelayTime = 1.0f;
    // the speed of the claw when cruising
    public float m_fCruiseSpeed = 25.0f;
    // the size multiplier when the claw is bigger
    public float m_fWeaponSizeMultiplier = 2.0f;
    // the health of the claw with the shield
    public int m_cShieldHealth = 6;
    // collection of materials
    public Material[] m_colours;
    // reference to the bottom light
    public GameObject m_bottomLight;
    // determines if the flashing should be constant or once off
    public bool m_bConstantFlashing = true;
    // reference to the reticle
    public GameObject m_reticle;

    // determines if the claw has dropped
    [HideInInspector]
    public bool m_bDropped = false;
    // determines if the claw picked up an item
    [HideInInspector]
    public bool m_bHasItem = false;
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
    // used to make sure that the bottom light only flashes once per player
    private bool[] m_bFlashed = new bool[4];
    // used to check if a bomb delay is on
    private bool m_bBombDelay = false;
    // checks if an audio is playing
    private bool m_bAudioPlaying = false;
    private float[] m_fOriginalVolumes = new float[2];

    private void Awake()
    {
        m_fOriginalVolumes[0] = GetComponents<AudioSource>()[0].volume;
        m_fOriginalVolumes[1] = GetComponents<AudioSource>()[1].volume;
    }

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

        for (int i = 0; i < m_bFlashed.Length; i++)
        {
            m_bFlashed[i] = false;
        }
    }

    private void Update()
    {
        // gets the sfx volume from the options
        if (OptionsManager.InstanceExists)
        {
            GetComponents<AudioSource>()[0].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume * m_fOriginalVolumes[0];
            GetComponents<AudioSource>()[1].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume * m_fOriginalVolumes[1];
        }

        m_bottomLight.GetComponent<Animator>().SetBool("bConstant", m_bConstantFlashing);

        // checks if there is someone in the crane
        if (m_crane.m_bOccupied)
        {
            LineRenderer line = GetComponentInChildren<LineRenderer>();
            line.SetPosition(1, new Vector3(0.0f, 0.0f, m_fLineLength));

            m_reticle.SetActive(true);

            // sets the colour of the line to the colour of the player
            if (CharacterManager.InstanceExists)
            {
                line.endColor = m_colours[CharacterManager.Instance.m_iDinoColours[m_crane.m_player.GetComponent<PlayerController>().m_cPlayerNumber - 1]].color;
            }

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
                    // stops the flashing animation
                    StopFlashing();
                    m_delay.bFlag = true;
                    if (m_bAudioPlaying)
                    {
                        GetComponents<AudioSource>()[0].Stop();
                        m_bAudioPlaying = false;
                    }
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
                    // stops the flashing animation
                    StopFlashing();
                    if (m_bAudioPlaying)
                    {
                        GetComponents<AudioSource>()[0].Stop();
                        m_bAudioPlaying = false;
                    }
                }
            }
        }
        else
        {
            // sets the line length to zero
            GetComponentInChildren<LineRenderer>().SetPosition(1, Vector3.zero);
            m_reticle.SetActive(false);
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
                if (m_bBombDelay)
                {
                    m_bBombDelay = false;
                    m_delay.fTimer = m_fBombDelayTime;
                }
                else
                {
                    m_delay.fTimer = m_fDelayTime;
                }
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
            if (!m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Play();
                m_bAudioPlaying = true;
            }
        }
        else
        {
            // creates an item at the claw
            m_item = Instantiate(m_itemTypes[0], transform);
            m_item.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            m_bHasItem = true;
            // sets the dropped status to true
            m_bDropped = true;
            if (m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Stop();
                m_bAudioPlaying = false;
            }
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
            if (!m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[1].Play();
                m_bAudioPlaying = true;
            }
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
            if (!m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[1].Play();
                m_bAudioPlaying = true;
            }
            return true;
        }
        else
        {
            if (m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Stop();
                m_bAudioPlaying = false;
            }
            // the claw has risen back to the top
            m_bDropped = false;
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

        // moves the crane relative to the way the player is facing
        if (m_crane.m_player.GetComponent<PlayerController>().m_bKonami)
        {
            v3Direction.x += m_crane.m_player.transform.forward.x * fVertical * Time.deltaTime;
            v3Direction.z += m_crane.m_player.transform.forward.z * fVertical * Time.deltaTime;
            v3Direction.x += m_crane.m_player.transform.right.x * fHorizontal * Time.deltaTime;
            v3Direction.z += m_crane.m_player.transform.right.z * fHorizontal * Time.deltaTime;
        }
        else
        {
            v3Direction.x = fHorizontal * Time.deltaTime;
            v3Direction.z = fVertical * Time.deltaTime;
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

            if (!m_bConstantFlashing)
            {
                if (!m_bFlashed[m_crane.m_player.GetComponent<PlayerController>().m_cPlayerNumber])
                {
                    if (!m_bottomLight.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FlashingLight") &&
                    !m_bottomLight.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FlashingLight 0"))
                    {
                        // makes the bottom light flash
                        m_bottomLight.GetComponent<Light>().color = GetComponentInChildren<LineRenderer>().endColor;
                        m_bottomLight.GetComponent<Animator>().SetTrigger("Flash");
                    }
                    m_bFlashed[m_crane.m_player.GetComponent<PlayerController>().m_cPlayerNumber] = true;
                }
            }
            else
            {
                if (!m_bottomLight.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FlashingLight") && 
                    !m_bottomLight.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FlashingLight 0"))
                {
                    // makes the bottom light flash
                    m_bottomLight.GetComponent<Light>().color = GetComponentInChildren<LineRenderer>().endColor;
                    m_bottomLight.GetComponent<Animator>().SetTrigger("Flash");
                }
            }

            if (m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Stop();
                m_bAudioPlaying = false;
            }
        }
        // checks if the claw collided with the platform
        else if (other.CompareTag("Ground"))
        {
            if (m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Stop();
                m_bAudioPlaying = false;
            }
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
            m_bHasItem = false;
            m_item.transform.parent = null;
            m_delay.bFlag = true;
            m_bBombDelay = true;
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
            if (!m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Play();
                m_bAudioPlaying = true;
            }
        }
        else
        {
            StopFlashing();

            // sets the dropped status to true
            m_bDropped = true;

            // get random block index
            int nBlockIndex = Random.Range(0, m_bombDropper.GetComponent<BombDropper>().m_blocks.Count);

            // get bomb position above random block
            Vector3 v3PlayerPos = m_bombDropper.GetComponent<BombDropper>().m_blocks[nBlockIndex].transform.position;
            v3PlayerPos.y = 5.0f;
            // moves the player from the crane back into the arena
            m_crane.m_player.transform.position = v3PlayerPos;
            m_crane.m_player.GetComponent<PlayerController>().m_bInCrane = false;
            m_crane.m_player.GetComponent<PlayerController>().m_claw = null;
            m_crane.m_player.GetComponent<PlayerController>().m_smokeParticle.Play();
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
            m_pickedUpPlayer.m_smokeParticle.Play();

            if (m_bAudioPlaying)
            {
                GetComponents<AudioSource>()[0].Stop();
                m_bAudioPlaying = false;
            }
        }
    }

    // stops the bottom light from flashing
    public void StopFlashing()
    {
        m_bottomLight.GetComponent<Animator>().SetTrigger("StopFlash");
    }
}