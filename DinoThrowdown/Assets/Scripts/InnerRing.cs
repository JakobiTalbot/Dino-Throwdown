using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerRing : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed = 25.0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.GetComponent<Claw>().m_fMoveSpeed = 40.0f;
        }
    }
}