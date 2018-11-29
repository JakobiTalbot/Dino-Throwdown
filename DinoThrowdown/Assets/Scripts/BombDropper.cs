using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BOMBDROPPER_STATE
{
    E_HASBOMB = 0,
    E_GETTINGBOMB,
    E_RESETTING
}
public class BombDropper : MonoBehaviour
{
    // reference to the game object that contains all the blocks
    public GameObject m_blocksParent;
    // reference to the block bomb prefab
    public GameObject m_bomb;
    // interval between bombs being dropped
    public float m_fDropInterval = 5.0f;
    // time before the bombs start dropping
    public float m_fSecondsUntilBombsStartDropping = 10.0f;
    // speed at which the claw moves
    public float m_fCraneMoveSpeed = 10.0f;
    // speed at which the claw drops
    public float m_fCraneDropSpeed = 10.0f;
    // the y value required to pick up a bomb
    public float m_fCraneDownYPosition = -10.0f;
    // range from the centre where the claw can move
    public float m_fOutPositionRange = 26.0f;
    // height required before dropping
    public int m_nDropHeight = 20;
    // maximum amount of blocks capable of being destroyed by a bomb
    public int m_nMaxBlocksToDestroy = 3;

    // reference to the currently held bomb
    [HideInInspector]
    public GameObject m_currentBomb;
    // used to time between drops
    [HideInInspector]
    public float m_fDropTimer;
    // collection of all the blocks' transforms
    [HideInInspector]
    public List<Transform> m_blocks;
    // used to time when the bombs should start dropping
    [HideInInspector]
    public float m_fTimeUntilBombsDrop;
    // initial y position of the claw
    [HideInInspector]
    public float m_fStartYPos;
    // enum to store state of bomb dropper crane
    [HideInInspector]
    public BOMBDROPPER_STATE m_state = BOMBDROPPER_STATE.E_RESETTING;

    // collection of the original positions of the blocks
    private Vector3[] m_v3OriginalBlockPositions;
    // place to pick up bombs from
    private Vector3 m_v3BombPickupPos;
    // place to drop bombs
    private Vector3 m_v3BombDropPos;
    // determines if the claw reached its desired location
    private bool m_bReachedLocation;

    private void Awake()
    {
        m_fTimeUntilBombsDrop = m_fSecondsUntilBombsStartDropping;
        // adds blocks to array of blocks
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
	
	private void Update()
    {
        // exits if the bombs have been toggled off
        if (OptionsManager.InstanceExists && !OptionsManager.Instance.m_bBombs)
            return;

        // decrement timer
        if (m_fTimeUntilBombsDrop > 0.0f)
            m_fTimeUntilBombsDrop -= Time.deltaTime;
        else
            m_fDropTimer -= Time.deltaTime;

        if (m_fDropTimer <= 0.0f && m_blocks.Count > 0)
        {
            switch (m_state)
            {
                case BOMBDROPPER_STATE.E_GETTINGBOMB:
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
                            m_currentBomb.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                            m_state = BOMBDROPPER_STATE.E_HASBOMB;
                            m_bReachedLocation = false;
                        }
                    }
                    break;

                case BOMBDROPPER_STATE.E_HASBOMB:
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
                            Vector3 v3DropDir = m_v3BombDropPos - transform.position;

                            if (v3DropDir.sqrMagnitude > 1.0f)
                            {
                                // seek drop location
                                transform.position += v3DropDir.normalized * m_fCraneMoveSpeed * Time.deltaTime;
                            }
                            else
                            {
                                // drop bomb
                                m_currentBomb.GetComponent<BlockBomb>().enabled = true;
                                m_currentBomb.GetComponent<Rigidbody>().isKinematic = false;
                                m_currentBomb.transform.parent = null;

                                m_state = BOMBDROPPER_STATE.E_RESETTING;
                                ResetTimers();
                            }
                        }
                    }
                    break;

                case BOMBDROPPER_STATE.E_RESETTING:
                    FindPosition();
                    break;
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
    public void ResetTimers()
    {
        m_fDropTimer = m_fDropInterval;
        m_fTimeUntilBombsDrop = m_fSecondsUntilBombsStartDropping;
    }

    public void FindPosition()
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
        m_state = BOMBDROPPER_STATE.E_GETTINGBOMB;
    }
}