using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Explosion : MonoBehaviour
{
    // the vibration time
    public float m_fVibrateTime = 0.5f;

    // used to time the vibration
    private float m_fVibrateTimer;
    // determines if the players are vibrating
    private bool m_bIsVibrating = false;
    // contains the player numbers of the players affected by the explosion
    private bool[] m_bPlayerNumbers;

    private void Awake()
    {
        m_fVibrateTimer = m_fVibrateTime;
        m_bPlayerNumbers = new bool[4];
    }

    private void Update()
    {
        if (m_bIsVibrating)
        {
            // decrement vibration timer
            m_fVibrateTimer -= Time.deltaTime;
            // turns off the vibration if the timer is less than zero
            if (m_fVibrateTimer <= 0.0f)
            {
                m_bIsVibrating = false;
                for (int i = 0; i < m_bPlayerNumbers.Length; i++)
                {
                    if (m_bPlayerNumbers[i])
                    {
                        GamePad.SetVibration((PlayerIndex)i, 0.0f, 0.0f);
                        m_fVibrateTimer = m_fVibrateTime;
                        m_bIsVibrating = true;
                    }
                }
            }
        }
    }

    public void SetVibration(bool[] bPlayerNumbers)
    {
        m_bPlayerNumbers = bPlayerNumbers;

        // turns on the vibration for the specific players
        if (!m_bIsVibrating)
        {
            for (int i = 0; i < m_bPlayerNumbers.Length; i++)
            {
                if (m_bPlayerNumbers[i])
                {
                    GamePad.SetVibration((PlayerIndex)i, 1.0f, 1.0f);
                    m_fVibrateTimer = m_fVibrateTime;
                    m_bIsVibrating = true;
                }
            }
        }
    }
}