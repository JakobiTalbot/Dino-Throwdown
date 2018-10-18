using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterRing : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Claw"))
        {
            other.transform.position = other.GetComponent<Claw>().m_v3PrevPos;
        }
    }
}