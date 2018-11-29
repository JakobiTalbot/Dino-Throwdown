using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Dash : MonoBehaviour
{
    public GameObject[] m_thrusters;
    public GameObject m_dashEffect;
    public Vector3 m_v3DashEffectPositionOffset;
    public float m_fYRotation = 90.0f;
    public float m_fDashVelocity = 50.0f;
    public float m_fDashCooldown = 5.0f;
    public float m_fDashVibrationTime = 0.5f;
    public float m_fDashVibrationStrength = 1.0f;

    private Rigidbody m_rigidbody;
    private float m_fTimer;
    private int m_cPlayerNumber;
    private bool m_bCanDash = true;
    private float m_fOriginalVolume = 1.0f;

	// Use this for initialization
	void Awake()
    {
        m_fTimer = m_fDashCooldown;
        m_rigidbody = GetComponent<Rigidbody>();

        m_cPlayerNumber = GetComponent<PlayerController>().m_cPlayerNumber;
        m_fOriginalVolume = GetComponents<AudioSource>()[4].volume;
    }
	
	// Update is called once per frame
	void Update()
    {
        // If dash cooldown is on
		if (!m_bCanDash)
        {
            // Increment timer
            m_fTimer -= Time.deltaTime;

            // Reset timer and allow dash after dash cooldown
            if (m_fTimer <= 0.0f)
            {
                ResetTimer();
            }

            foreach (var thruster in m_thrusters)
            {
                thruster.GetComponent<ParticleSystem>().Stop();
            }
        }
        else
        {
            foreach (var thruster in m_thrusters)
            {
                thruster.GetComponent<ParticleSystem>().Play();
            }
        }
	}

    public void DoDash()
    {
        if (m_bCanDash)
        {
            // get controller input
            GamePadState gamePadState = GamePad.GetState((PlayerIndex)m_cPlayerNumber - 1);
            // get input vector2
            Vector2 v2LSInput = new Vector2(gamePadState.ThumbSticks.Left.X + Input.GetAxis("Horizontal" + m_cPlayerNumber), gamePadState.ThumbSticks.Left.Y + Input.GetAxis("Vertical" + m_cPlayerNumber));
            v2LSInput.Normalize();

            if (v2LSInput == Vector2.zero)
                return;

            m_rigidbody.velocity = Vector3.zero;

            // add dash velocity
            m_rigidbody.velocity += new Vector3(v2LSInput.x * m_fDashVelocity, 0.0f, v2LSInput.y * m_fDashVelocity);

            // set vibration
            GetComponent<PlayerController>().SetVibration(m_fDashVibrationTime, m_fDashVibrationStrength);

            // create dash particles
            GameObject newParticles = Instantiate(m_dashEffect, transform);
            newParticles.transform.localPosition = m_v3DashEffectPositionOffset;
            newParticles.transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.y + 180.0f, 0.0f);


            // gets the sfx volume from the options
            if (OptionsManager.InstanceExists)
            {
                GetComponents<AudioSource>()[4].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume * m_fOriginalVolume;
            }
            // play dash sound
            GetComponents<AudioSource>()[4].Play();
            m_bCanDash = false;
        }
    }

    public float GetCooldown()
    {
        return m_fTimer;
    }

    public void ResetTimer()
    {
        m_bCanDash = true;
        m_fTimer = m_fDashCooldown;
    }
}