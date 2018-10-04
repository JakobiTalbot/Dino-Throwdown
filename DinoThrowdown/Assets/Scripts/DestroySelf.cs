using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float m_timeToDestroy = 5.0f;

    private float m_timer = 0.0f;
	
	// Update is called once per frame
	void Update()
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_timeToDestroy)
        {
            Destroy(gameObject);
        }
	}
}
