using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : MonoBehaviour
{
    public GameObject m_blocksParent;
    public GameObject m_bomb;
    public float m_fDropInterval = 5.0f;
    public int m_nDropHeight = 20;
    public int m_nMaxBlocksToDestroy = 3;

    [HideInInspector]
    public float m_fDropTimer;
    [HideInInspector]
    public List<Transform> m_blocks;

    private Vector3[] m_v3OriginalBlockPositions;

    // Use this for initialization
    void Awake()
    {
        AddBlocks();
        m_v3OriginalBlockPositions = new Vector3[m_blocks.Count];

        // get block positions
        for (int i = 0; i < m_blocks.Count; ++i)
        {
            m_v3OriginalBlockPositions[i] = m_blocks[i].transform.position;
        }

        m_fDropTimer = m_fDropInterval;
	}
	
	// Update is called once per frame
	void Update()
    {
        // decrement timer
        m_fDropTimer -= Time.deltaTime;

        if (m_fDropTimer <= 0.0f && m_blocks.Count > 0)
        {
            // get random block index
            int nBlockIndex = Random.Range(0, m_blocks.Count);

            // get bomb position above random block
            Vector3 v3BombPos = m_blocks[nBlockIndex].transform.position;
            v3BombPos.y = m_nDropHeight;

            // create bomb above random block
            GameObject bomb = Instantiate(m_bomb, v3BombPos, Quaternion.Euler(Vector3.zero));
            bomb.GetComponent<BlockBomb>().m_nMaxBlocksToDestroy = m_nMaxBlocksToDestroy;
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.GetComponent<SphereCollider>().material = null;

            // reset timer
            m_fDropTimer = m_fDropInterval;
        }
	}

    // adds blocks to array of blocks
    public void AddBlocks()
    {
        for (int i = 1; i < m_blocksParent.GetComponentsInChildren<Transform>().Length; ++i)
        {
            m_blocks.Add(m_blocksParent.GetComponentsInChildren<Transform>()[i]);
        }
    }

    // resets block position/rotation/kinematic, call after AddBlocks()
    public void ResetBlocks()
    {
        for (int i = 0; i < m_blocks.Count; ++i)
        {
            m_blocks[i].position = m_v3OriginalBlockPositions[i];
            m_blocks[i].rotation = Quaternion.Euler(Vector3.zero);
            m_blocks[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}