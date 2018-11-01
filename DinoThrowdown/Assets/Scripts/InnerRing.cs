using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRing : MonoBehaviour
{
    // claw speed multiplier
    public float m_fSpeedMultiplier = 4.0f;
    // claw line length multiplier
    public float m_fLengthMultiplier = 3.0f;

    // sets the speed and length of the claw values back to normal
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            if (other.GetComponent<Claw>().m_bFirstFrame)
            {
                other.GetComponent<Claw>().m_bFirstFrame = false;
            }
            else
            {
                other.GetComponent<Claw>().m_fMoveSpeed /= m_fSpeedMultiplier;
                other.GetComponent<Claw>().m_fLineLength /= m_fLengthMultiplier;
            }
        }
    }
    // increases the speed and length of the claw values
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed *= m_fSpeedMultiplier;
            other.GetComponent<Claw>().m_fLineLength *= m_fLengthMultiplier;
        }
    }
}