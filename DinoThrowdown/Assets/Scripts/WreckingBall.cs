using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    public float m_fSwingTimeToWait = 10.0f;
    public float m_fSwingSpeed = 0.1f;

    private Quaternion m_SwingDir = new Quaternion();
    private float m_fSwingTimer;
    private bool m_bSwinging;

	// Use this for initialization
	void Awake()
    {
        m_fSwingTimer = m_fSwingTimeToWait;
	}
	
	// Update is called once per frame
	void Update()
    {
        m_fSwingTimer -= Time.deltaTime;
        Debug.Log(m_fSwingTimer);

        if (m_fSwingTimer <= 0.0f && !m_bSwinging)
        {
            Vector3 v3RandDir = new Vector3()
            {
                x = Random.Range(-90.0f, 90.0f),
                z = Random.Range(-90.0f, 90.0f)
            };
            m_SwingDir.eulerAngles = v3RandDir;

            Quaternion swingOrigin = m_SwingDir;
            swingOrigin.eulerAngles *= -1.0f;
            transform.rotation = swingOrigin;

            m_bSwinging = true;
        }

        if (m_bSwinging)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, m_SwingDir, m_fSwingSpeed);

            if ((transform.rotation.eulerAngles - m_SwingDir.eulerAngles).magnitude < 5.0f)
            {
                m_fSwingTimer = m_fSwingTimeToWait;
                m_bSwinging = false;
            }
        }
	}
}