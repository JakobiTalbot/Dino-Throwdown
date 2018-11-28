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
    public float m_fDashForce = 1000.0f;
    public float m_fDashCooldown = 5.0f;
    public float m_fDashVibrationTime = 0.5f;
    public float m_fDashVibrationStrength = 1.0f;

    private Rigidbody m_rigidbody;
    private float m_fTimer;
    private int m_cPlayerNumber;
    private bool m_bCanDash = true;

	// Use this for initialization
	void Awake()
    {
        m_fTimer = m_fDashCooldown;
        m_rigidbody = GetComponent<Rigidbody>();

        m_cPlayerNumber = GetComponent<PlayerController>().m_cPlayerNumber;
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
            m_rigidbody.velocity = Vector3.zero;
            GamePadState m_gamePadState = GamePad.GetState((PlayerIndex)m_cPlayerNumber - 1);
            // Get position for explosive force
            Vector3 v3ExplosionPos = transform.position;
            v3ExplosionPos.x -= 0.5f; // because the transform.position isn't the centre of the model
            v3ExplosionPos.x -= (Input.GetAxis("Horizontal" + m_cPlayerNumber) + m_gamePadState.ThumbSticks.Left.X) * 3.0f;
            v3ExplosionPos.y = transform.position.y + 1.0f; // So the player doesn't dash up
            v3ExplosionPos.z -= (Input.GetAxis("Vertical" + m_cPlayerNumber) + m_gamePadState.ThumbSticks.Left.Y) * 3.0f;

            // set vibration
            GetComponent<PlayerController>().SetVibration(m_fDashVibrationTime, m_fDashVibrationStrength);

            // create dash particles
            GameObject newParticles = Instantiate(m_dashEffect, transform);
            newParticles.transform.localPosition = m_v3DashEffectPositionOffset;
            newParticles.transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.y + 180.0f, 0.0f);

            // add dash force
            m_rigidbody.AddExplosionForce(m_fDashForce, v3ExplosionPos, 20.0f);

            // gets the sfx volume from the options
            if (OptionsManager.InstanceExists)
            {
                GetComponents<AudioSource>()[4].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
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