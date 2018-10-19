using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // speed that the light increases and decreases
    public float m_fExplosionSpeed = 1.0f;
    // the range of the explosion
    public float m_fExplosionRange = 2.0f;

    // reference to the light source
    private Light m_light;
    // used to increase the light intensity
    private float m_timer = 0.0f;

    private void Awake()
    {
        m_light = GetComponent<Light>();
    }

    private void Update()
    {
        // increments the time pased
        m_timer += Time.deltaTime * m_fExplosionSpeed;
        // ensures that the timer is relevant
        Mathf.Clamp(m_timer, 0.0f, m_fExplosionRange);
        // sets the intensity of the light to the timer
        m_light.intensity = m_timer;

        // checks if the intensity is above the range and that the speed has not yet been inverted
        if (m_light.intensity >= m_fExplosionRange && m_fExplosionSpeed >= 0.0f)
        {
            // inverts the speed so that the light decreases
            m_fExplosionSpeed *= -1.0f;
        }
    }
}