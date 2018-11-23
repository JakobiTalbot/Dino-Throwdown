using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : MonoBehaviour
{
    public GameObject m_blocksParent;
    public GameObject m_bomb;
    public float m_fDropInterval = 5.0f;
    public float m_fSecondsUntilBombsStartDropping = 10.0f;
    public float m_fCraneMoveSpeed = 10.0f;
    public float m_fCraneDropSpeed = 10.0f;
    public float m_fCraneDownYPosition = -10.0f;
    public float m_fOutPositionRange = 26.0f;
    public int m_nDropHeight = 20;
    public int m_nMaxBlocksToDestroy = 3;

    [HideInInspector]
    public GameObject m_currentBomb;
    [HideInInspector]
    public float m_fDropTimer;
    [HideInInspector]
    public List<Transform> m_blocks;
    [HideInInspector]
    public float m_fTimeUntilBombsDrop;
    [HideInInspector]
    public bool m_bHasBomb;
    [HideInInspector]
    public bool m_bGettingBomb;
    [HideInInspector]
    public float m_fStartYPos;

    private Vector3[] m_v3OriginalBlockPositions;
    private Vector3 m_v3BombPickupPos;
    private Vector3 m_v3BombDropPos;
    private bool m_bReachedLocation;


    // Use this for initialization
    void Awake()
    {
        m_fTimeUntilBombsDrop = m_fSecondsUntilBombsStartDropping;
        AddBlocks();
        m_v3OriginalBlockPositions = new Vector3[m_blocks.Count];

        // get block positions
        for (int i = 0; i < m_blocks.Count; ++i)
        {
            m_v3OriginalBlockPositions[i] = m_blocks[i].transform.position;
        }
        m_fStartYPos = transform.position.y;
        m_fDropTimer = m_fDropInterval;
	}
	
	// Update is called once per frame
	void Update()
    {
        if (OptionsManager.InstanceExists && !OptionsManager.Instance.m_bBombs)
            return;

        // decrement timer
        if (m_fTimeUntilBombsDrop > 0.0f)
            m_fTimeUntilBombsDrop -= Time.deltaTime;
        else
            m_fDropTimer -= Time.deltaTime;

        if (m_fDropTimer <= 0.0f && m_blocks.Count > 0)
        {
            if (m_bHasBomb)
            {
                // if crane is down
                if (transform.position.y < m_fStartYPos)
                {
                    // move crane up
                    transform.position += Vector3.up * m_fCraneDropSpeed * Time.deltaTime;
                }
                else
                {
                    // get direction to drop location
                    Vector3 v3Dir = m_v3BombDropPos - transform.position;

                    if (v3Dir.sqrMagnitude > 1.0f)
                    {
                        // seek drop location
                        transform.position += v3Dir.normalized * m_fCraneMoveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        // drop bomb
                        m_currentBomb.GetComponent<BlockBomb>().enabled = true;
                        m_currentBomb.GetComponent<Rigidbody>().isKinematic = false;
                        m_currentBomb.transform.parent = null;

                        m_bHasBomb = false;
                        ResetTimer();
                    }
                }
            }
            else if (m_bGettingBomb)
            {
                Vector3 v3Dir = m_v3BombPickupPos - transform.position;
                if ((m_v3BombPickupPos - transform.position).sqrMagnitude > 1.0f && !m_bReachedLocation)
                {
                    // move in direction
                    transform.position += v3Dir.normalized * m_fCraneMoveSpeed * Time.deltaTime;
                }
                else
                {
                    m_bReachedLocation = true;
                    // if crane is not down
                    if (transform.position.y >= m_fCraneDownYPosition)
                    {
                        // move down
                        transform.position += Vector3.down * m_fCraneMoveSpeed * Time.deltaTime;
                    }
                    else //if crane is down
                    {
                        // get random block index
                        int nBlockIndex = Random.Range(0, m_blocks.Count);
                        // get bomb position above random block
                        m_v3BombDropPos = m_blocks[nBlockIndex].transform.position;
                        m_v3BombDropPos.y = m_nDropHeight;

                        // create bomb
                        m_currentBomb = Instantiate(m_bomb, transform);
                        m_currentBomb.GetComponent<BlockBomb>().m_nMaxBlocksToDestroy = m_nMaxBlocksToDestroy;
                        m_currentBomb.GetComponent<BlockBomb>().m_bombDropper = gameObject;
                        m_currentBomb.GetComponent<BlockBomb>().enabled = false;
                        m_bHasBomb = true;
                        m_bGettingBomb = false;
                        m_bReachedLocation = false;
                    }
                }
            }
            else
            {
                // create random position
                Vector3 v3RandPos = new Vector3();
                v3RandPos.y = m_fStartYPos;

                int nGrabbingAxis = Random.Range(0, 1);
                int nNegative = Random.Range(0, 1);
                if (nNegative == 0)
                    nNegative = -1;
                switch (nGrabbingAxis)
                {
                    case 0:
                        v3RandPos.x = m_fOutPositionRange * nNegative;
                        v3RandPos.z = Random.Range(-20.0f, 20.0f);
                        break;
                    case 1:
                        v3RandPos.x = Random.Range(-20.0f, 20.0f);
                        v3RandPos.z = m_fOutPositionRange * nNegative;
                        break;
                }

                // set seek location
                m_v3BombPickupPos = v3RandPos;
                m_bGettingBomb = true;
            }
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

    // reset timer
    public void ResetTimer()
    {
        m_fDropTimer = m_fDropInterval;
    }
}