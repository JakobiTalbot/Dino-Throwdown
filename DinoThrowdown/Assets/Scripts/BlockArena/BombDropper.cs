using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : MonoBehaviour
{
    public GameObject m_bomb;
    public float m_fDropInterval = 5.0f;
    public float m_fDropHeight = 30.0f;
    public int m_nMaxBlocksToDestroy = 3;

    [HideInInspector]
    public float m_fDropTimer;
	// Use this for initialization
	void Awake()
    {
        m_fDropTimer = m_fDropInterval;	
	}
	
	// Update is called once per frame
	void Update()
    {
        // decrement timer
        m_fDropTimer -= Time.deltaTime;

        if (m_fDropTimer <= 0.0f)
        {
            // get random position
            Vector3 v3RandPos = new Vector3()
            {
                x = Random.Range(-42.0f, 42.0f),
                y = m_fDropHeight,
                z = Random.Range(-42.0f, 42.0f)
            };

            // create bomb at random position
            GameObject bomb = Instantiate(m_bomb, v3RandPos, Quaternion.Euler(Vector3.zero));
            bomb.GetComponent<BlockBomb>().m_nMaxBlocksToDestroy = m_nMaxBlocksToDestroy;
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.GetComponent<SphereCollider>().material = null;

            // reset timer
            m_fDropTimer = m_fDropInterval;
        }

	}
}