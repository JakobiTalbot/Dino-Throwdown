using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
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
    public int m_cClawHealth = 5;

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

    // reference to the crane
    private CraneManager m_crane;
    private bool m_bClawDownSoundPlayed = false;
    private bool m_bClawUpSoundPlayed = false;
    // reference to the player being picked up
    private PlayerController m_pickedUpPlayer = null;

    private void Start()
    {
        m_crane = GetComponentInParent<CraneManager>();
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

            if (m_bHasPlayer)
            {
                if (m_pickedUpPlayer.m_cAttackAmount == m_cClawHealth)
                {
                    m_pickedUpPlayer.m_cAttackAmount = 0;
                    m_pickedUpPlayer.GetComponent<Rigidbody>().isKinematic = false;
                    m_bHasPlayer = false;
                    transform.position = new Vector3(transform.position.x, 16.0f, transform.position.z);
                }
            }
        }
        else
        {
            // sets the line length to zero
            GetComponentInChildren<LineRenderer>().SetPosition(1, Vector3.zero);
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
            if (!m_bClawDownSoundPlayed)
                GetComponents<AudioSource>()[0].Play();
            m_bClawDownSoundPlayed = true;
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
            if (!m_bClawUpSoundPlayed)
                GetComponents<AudioSource>()[1].Play();
            m_bClawUpSoundPlayed = true;
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
            m_bClawDownSoundPlayed = false;
            m_bClawUpSoundPlayed = false;
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

        // moves the claw by the direction
        transform.Translate(v3Direction * fSpeed);
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
        if (other.CompareTag("Player") && !other.GetComponent<PlayerController>().m_bInCrane)
        {
            m_bHasPlayer = true;
            // sets the claw to have dropped
            m_bDropped = true;
            m_pickedUpPlayer = other.GetComponent<PlayerController>();
            m_pickedUpPlayer.GetComponent<Rigidbody>().isKinematic = true;

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

            m_crane.m_player.transform.position = new Vector3(0.0f, 5.0f, 0.0f);
            m_crane.m_player.GetComponent<PlayerController>().m_bInCrane = false;
            m_crane.m_player.GetComponent<PlayerController>().m_claw = null;
            m_crane.m_player.GetComponent<Rigidbody>().isKinematic = false;

            transform.position = new Vector3(0.0f, 16.0f, 0.0f);
            m_bHasPlayer = false;

            // sets this player as the one in the crane
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

            // sets the status as not out or picked up and in the crane
            m_pickedUpPlayer.m_bIsOut = false;
            m_pickedUpPlayer.m_bInCrane = true;
            m_pickedUpPlayer.m_claw = this;
            m_pickedUpPlayer.m_cAttackAmount = 0;
        }
    }
}