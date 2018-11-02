using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    // time to wait before swinging again after finishing last swing
    public float m_fSwingTimeToWait = 10.0f;
    //// speed to swing
    //public float m_fSwingSpeed = 100.0f;
    public LineRenderer m_warningLine;

    private float m_fSwingTimer;
    private Quaternion m_swingEnd;
    private Vector3 m_v3SwingDir;
    private bool m_bSwinging = true;

	// Use this for initialization
	void Awake()
    {
        // set swing timer
        m_fSwingTimer = m_fSwingTimeToWait;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bSwinging)
        {
            m_fSwingTimer -= Time.deltaTime;
            if (m_fSwingTimer <= 0)
            {
                float fRandYRotation = Random.Range(0.0f, 360.0f);
            }
        }

        //// decrement swing timer
        //m_fSwingTimer -= Time.deltaTime;

        //if (m_fSwingTimer <= 0.0f && !m_bSwinging)
        //{
        //    // get random y rotation
        //    float fRandY = Random.Range(0.0f, 360.0f);

        //    // set end rotation
        //    m_swingEnd = Quaternion.Euler(0.0f, fRandY, 90.0f);
        //    // set start rotation
        //    transform.rotation = Quaternion.Euler(0.0f, fRandY, -90.0f);

        //    // find swing direction
        //    m_v3SwingDir = (transform.rotation.eulerAngles - m_swingEnd.eulerAngles).normalized;
        //    m_bSwinging = true;

        //    m_warningLine.GetComponent<LineRenderer>().enabled = true;
        //    // set line rotation
        //    m_warningLine.transform.rotation = Quaternion.Euler(0.0f, fRandY, 0.0f);
        //}

        //if (m_bSwinging)
        //{
        //    // rotate wrecking ball (swing)
        //    transform.Rotate(m_v3SwingDir * m_fSwingSpeed * Time.deltaTime);

        //    // end swing if close to end rotation
        //    if ((m_swingEnd.eulerAngles - transform.rotation.eulerAngles).sqrMagnitude < 50.0f)
        //    {
        //        m_bSwinging = false;
        //        m_fSwingTimer = m_fSwingTimeToWait;
        //        m_warningLine.GetComponent<LineRenderer>().enabled = false;
        //    }
        //}
	}
}