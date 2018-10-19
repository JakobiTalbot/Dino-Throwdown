using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRing : MonoBehaviour
{
    // new speed of the claw
    public float m_fIncreasedSpeed = 40.0f;
    // new length of the line
    public float m_fIncreasedLength = 30.0f;

    // original speed of the claw
    private float m_fOriginalSpeed = 0.0f;
    // original length of the line
    private float m_fOriginalLength = 0.0f;

    private void Start()
    {
        // gets the original values
        m_fOriginalSpeed = GameObject.FindGameObjectWithTag("Claw").GetComponent<Claw>().m_fMoveSpeed;
        m_fOriginalLength = GameObject.FindGameObjectWithTag("Claw").GetComponent<Claw>().m_fLineLength;
    }

    // sets the speed and length of the claw values back to normal
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed = m_fOriginalSpeed;
            other.GetComponent<Claw>().m_fLineLength = m_fOriginalLength;
        }
    }
    // increases the speed and length of the claw values
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed = m_fIncreasedSpeed;
            other.GetComponent<Claw>().m_fLineLength = m_fIncreasedLength;
        }
    }
}