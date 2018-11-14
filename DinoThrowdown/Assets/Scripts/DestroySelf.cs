using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float m_nTimeToDestroy = 5.0f;
	
	private void Update()
    {
        m_nTimeToDestroy -= Time.deltaTime;

        if (m_nTimeToDestroy <= 0.0f)
        {
            Destroy(gameObject);
        }
	}
}
