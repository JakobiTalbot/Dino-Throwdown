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

    // reference to the crane
    private CraneManager m_crane;

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
            switch (m_crane.m_player.GetComponent<PlayerController>().m_cPlayerNumber)
            {
                case 1:
                    line.endColor = new Color(0.0f, 0.5f, 1.0f);
                    break;
                case 2:
                    line.endColor = Color.red;
                    break;
                case 3:
                    line.endColor = Color.green;
                    break;
                case 4:
                    line.endColor = Color.yellow;
                    break;
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
        // check is the claw is below the target height
        if (transform.localPosition.y < 10.0f)
        {
            // moves the claw up
            transform.Translate(transform.up * Time.deltaTime * m_fMoveSpeed);

            return true;
        }
        else
        {
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
            // sets the claw to have dropped
            m_bDropped = true;
            // gets the player controller from the player
            PlayerController playerController = other.GetComponent<PlayerController>();
            // sets the player to picked up
            playerController.m_bPickedUp = true;
            // gives the player control over the claw
            playerController.m_claw = gameObject.GetComponent<Claw>();

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
}