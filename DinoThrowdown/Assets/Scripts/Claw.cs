using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    // speed at which the claw drops
    public float m_fMoveSpeed = 10.0f;
    // types of items to pick up
    public GameObject[] m_itemTypes;

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

        if (m_bItemDrop)
        {
            m_item.transform.position = new Vector3(v3PrevPos.x, m_item.transform.position.y, v3PrevPos.z);
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