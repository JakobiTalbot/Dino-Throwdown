using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRing : MonoBehaviour
{
    // new speed of the claw
    public float m_fIncreasedSpeed = 40.0f;

    // original speed of the claw
    private float m_fOriginalSpeed = 0.0f;

    private void Start()
    {
        // gets the original speed
        m_fOriginalSpeed = GameObject.FindGameObjectWithTag("Claw").GetComponent<Claw>().m_fMoveSpeed;
    }

    // sets the speed back to normal
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed = m_fOriginalSpeed;
        }
    }
    // increases the speed
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed = m_fIncreasedSpeed;
        }
    }
}