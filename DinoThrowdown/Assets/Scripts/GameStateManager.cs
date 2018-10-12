using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public int m_nRoundsToWin = 3;
    public GameObject[] m_players;

    private List<GameObject> m_playersRemaining;
    private int[] m_nRoundsWon;
    private int m_nRoundNumber = 0;
	// Use this for initialization
	void Awake()
    {
		for (int i = 0; i < m_players.Length; ++i)
        {
            m_nRoundsWon[i] = 0;
            m_playersRemaining.Add(m_players[i]);
        }
	}
	
	// Update is called once per frame
	void Update()
    {
		for (int i = 0; i < m_playersRemaining.Count; ++i)
        {
            if (m_playersRemaining[i].GetComponent<PlayerController>().m_bIsOut)
            {
                m_playersRemaining.RemoveAt(i);
            }
        }

        if (m_playersRemaining.Count == 1)
        {
            for (int i = 0; i < m_players.Length; ++i)
            {
                if (m_playersRemaining[0] == m_players[i])
                {
                    ++m_nRoundsWon[i];

                    if (m_nRoundsWon[i] == m_nRoundsToWin)
                    {
                        // player wins
                    }
                    else
                    {
                        NewRound();
                    }
                }
            }
        }
	}

    private void NewRound()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            m_nRoundsWon[i] = 0;
            m_playersRemaining.Add(m_players[i]);

            // reset player positions/knockback/powerups here
        }
    }
}