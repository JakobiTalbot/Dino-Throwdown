using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : MonoBehaviour
{
    public GameObject m_blocksParent;
    public GameObject m_bomb;
    public float m_fDropInterval = 5.0f;
    public float m_fDropHeight = 30.0f;
    public int m_nMaxBlocksToDestroy = 3;

    [HideInInspector]
    public float m_fDropTimer;
    [HideInInspector]
    public List<GameObject> m_blocks;
	// Use this for initialization
	void Awake()
    {
        // add blocks to array
        for (int i = 0; i < m_blocksParent.GetComponentsInChildren<GameObject>().Length; ++i)
        {
            m_blocks.Add(m_blocksParent.GetComponentsInChildren<GameObject>()[i]);
        }
        m_fDropTimer = m_fDropInterval;
	}
	
	// Update is called once per frame
	void Update()
    {
        // decrement timer
        m_fDropTimer -= Time.deltaTime;

        if (m_fDropTimer <= 0.0f)
        {


            // create bomb at random position
            //GameObject bomb = Instantiate(m_bomb, v3RandPos, Quaternion.Euler(Vector3.zero));
            //bomb.GetComponent<BlockBomb>().m_nMaxBlocksToDestroy = m_nMaxBlocksToDestroy;
            //bomb.GetComponent<Rigidbody>().isKinematic = false;
            //bomb.GetComponent<SphereCollider>().material = null;

            // reset timer
            m_fDropTimer = m_fDropInterval;
        }

	}
}