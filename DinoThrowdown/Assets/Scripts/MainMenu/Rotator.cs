using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // rotation speed
    public float m_fRotateSpeed = 20.0f;

    private void Update()
    {
        // rotates the object around the y axis by the speed
        transform.Rotate(0.0f, m_fRotateSpeed * Time.deltaTime, 0.0f);
    }
}