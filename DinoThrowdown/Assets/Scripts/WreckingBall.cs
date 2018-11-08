using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    // time to wait before swinging again after finishing last swing
    public float m_fSwingWaitTime = 10.0f;
    // time to wait before swinging starts
    public float m_fStartWaitTime = 20.0f;

    // determines if the wrecking ball is swinging
    [HideInInspector]
    public bool m_bSwinging;

    // used to time the wrecking ball
    private float m_fSwingTimer;
    // collection of references to wrecking ball chain components
    private CapsuleCollider[] m_chain;
    // reference to the wrecking ball holder
    private BoxCollider m_holder;
    // reference to the ball
    private SphereCollider m_ball;
    // reference to the warning line
    private LineRenderer m_warningLine;
    // reference to the animator for controlling the warning line
    private Animator m_anim;

	private void Awake()
    {
        // gets the wrecking ball components
        m_chain = GetComponentsInChildren<CapsuleCollider>();
        m_holder = GetComponentInChildren<BoxCollider>();
        m_ball = GetComponentInChildren<SphereCollider>();
        m_warningLine = GetComponentInChildren<LineRenderer>();
        m_anim = GetComponent<Animator>();

        ResetBall();
    }

    private void Update()
    {
        // decrements the timer
        m_fSwingTimer -= Time.deltaTime;

        // resets the animation
        if (!m_anim.GetBool("bIsSwinging") && m_bSwinging)
        {
            m_anim.SetBool("bIsSwinging", true);
        }

        // checks if the timer has run out
        if (m_fSwingTimer <= 0)
        {
            // checks if the wrecking ball should be swinging
            if (m_bSwinging)
            {
                // chooses a random y rotation
                float fRandYRotation = Random.Range(0.0f, 360.0f);
                // resets the timer
                m_fSwingTimer = m_fSwingWaitTime;
                m_anim.SetBool("bIsSwinging", false);

                // sets the wrecking ball y rotation to the random y rotation
                transform.rotation = Quaternion.Euler(0.0f, fRandYRotation, 0.0f);
                // sets the rotation of the warning line
                m_warningLine.transform.rotation = Quaternion.Euler(0.0f, fRandYRotation, 0.0f);

                // moves all the chain links to the correct transform and resets their velocities
                for (int i = 0; i < m_chain.Length; i++)
                {
                    m_chain[i].transform.localPosition = new Vector3(0.0f, 0.0f, 2.5f + ((float)i * 4.0f));
                    m_chain[i].transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                    m_chain[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                    m_chain[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }

                // moves the wrecking ball holder to the initial transform and velocity
                m_holder.transform.localPosition = Vector3.zero;
                m_holder.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                m_holder.GetComponent<Rigidbody>().velocity = Vector3.zero;
                m_holder.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                // moves the ball to the initial transform and velocity
                m_ball.transform.localPosition = new Vector3(0.0f, 0.0f, 41.5f);
                m_ball.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                m_ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                m_ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            else
            {
                // sets the wrecking ball up to start swinging
                m_bSwinging = true;
                m_anim.SetBool("bIsSwinging", true);

                // sets all the wrecking ball components to not kinematic
                foreach (var chain in m_chain)
                {
                    chain.GetComponent<Rigidbody>().isKinematic = false;
                }

                m_holder.GetComponent<Rigidbody>().isKinematic = false;
                m_ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
	}

    // resets the wrecking ball
    public void ResetBall()
    {
        // resets the time and animation
        m_fSwingTimer = m_fStartWaitTime;
        m_bSwinging = false;
        m_anim.SetBool("bIsSwinging", false);

        // sets the wrecking ball and warning line to inital rotations
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        m_warningLine.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        // moves all the chain links to the correct transform and resets their velocities
        for (int i = 0; i < m_chain.Length; i++)
        {
            m_chain[i].transform.localPosition = new Vector3(0.0f, 0.0f, 2.5f + ((float)i * 4.0f));
            m_chain[i].transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            m_chain[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_chain[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            m_chain[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        // moves the wrecking ball holder to the initial transform and velocity
        m_holder.transform.localPosition = Vector3.zero;
        m_holder.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        m_holder.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_holder.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        m_holder.GetComponent<Rigidbody>().isKinematic = true;

        // moves the ball to the initial transform and velocity
        m_ball.transform.localPosition = new Vector3(0.0f, 0.0f, 41.5f);
        m_ball.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        m_ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        m_ball.GetComponent<Rigidbody>().isKinematic = true;
    }
}