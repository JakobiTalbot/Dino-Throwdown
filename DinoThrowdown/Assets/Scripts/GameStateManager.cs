using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public int m_nRoundsToWin = 3;
    public GameObject[] m_players;

    private GameObject m_canvas;
    private List<GameObject> m_playersRemaining;
    private Vector3[] m_originalPositions;
    private Quaternion[] m_originalRotations;
    private UI m_UI;
    private float m_fVictoryWaitTime = 5.0f;
    private int[] m_nRoundsWon;
    private int m_nRoundNumber = 1;
    private bool m_bPlayerWon = false;

	// Use this for initialization
	void Awake()
    {
        m_originalPositions = new Vector3[m_players.Length];
        m_originalRotations = new Quaternion[m_players.Length];
        m_playersRemaining = new List<GameObject>();
        m_nRoundsWon = new int[m_players.Length];
        m_canvas = GameObject.FindGameObjectWithTag("Canvas");

        for (int i = 0; i < m_players.Length; ++i)
        {
            // set each player's rounds won to 0
            m_nRoundsWon[i] = 0;

            // add each player to remaining players
            m_playersRemaining.Add(m_players[i]);

            // get original positions/rotations of each player
            m_originalPositions[i] = m_players[i].transform.position;
            m_originalRotations[i] = m_players[i].transform.rotation;
        }
	}
	
	// Update is called once per frame
	void Update()
    {
        Debug.Log(m_playersRemaining.Count);
        // if there is not yet a winner
        if (!m_bPlayerWon)
        {
            for (int i = 0; i < m_playersRemaining.Count; ++i)
            {
                // remove player from players remaining if they are out
                if (m_playersRemaining[i].GetComponent<PlayerController>().m_bIsOut
                    || m_playersRemaining[i].GetComponent<PlayerController>().m_bInCrane)
                {
                    m_playersRemaining.RemoveAt(i);
                }
            }

            // if there is only one player left
            if (m_playersRemaining.Count == 1)
            {
                for (int i = 0; i < m_players.Length; ++i)
                {
                    // find which player it was
                    if (m_playersRemaining[0] == m_players[i])
                    {
                        // increment rounds won
                        ++m_nRoundsWon[i];
                        //m_canvas.GetComponent<UI>().EnableRoundImage(m_players[i]);

                        // check if they won the amount of rounds needed to win game
                        if (m_nRoundsWon[i] == m_nRoundsToWin)
                        {
                            m_bPlayerWon = true;
                            //m_canvas.GetComponent<UI>().EnablePlayerWon(m_players[i]);
                        }
                        else
                        {
                            NewRound();
                        }
                    }
                }
            }
        }
        else
        {
            // PLAYER WON SCREEN
            m_fVictoryWaitTime -= Time.deltaTime;

            if (m_fVictoryWaitTime <= 0.0f)
            {
                // go back to main menu
            }
        }
	}

    private void NewRound()
    {
        m_playersRemaining.Clear();
        for (int i = 0; i < m_players.Length; ++i)
        {
            // increment round number
            ++m_nRoundNumber;
            // add players to remaining players
            m_playersRemaining.Add(m_players[i]);

            // reset player position
            m_players[i].transform.position = m_originalPositions[i];
            // reset player rotation
            m_players[i].transform.rotation = m_originalRotations[i];
            // reset player velocities
            m_players[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_players[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            // reset player knockback meter
            m_players[i].GetComponent<Knockback>().m_fKnockbackMeter = 0.0f;
            // reset powerups
            m_players[i].GetComponent<PlayerController>().m_cruiseControl.bFlag = false;
            m_players[i].GetComponent<PlayerController>().m_cruiseControl.fTimer = 0.0f;
            m_players[i].GetComponent<Knockback>().m_shield.bFlag = false;
            m_players[i].GetComponent<Knockback>().m_shield.fTimer = 0.0f;
            m_players[i].GetComponent<PlayerController>().StopAttack();
            // reset player status
            m_players[i].GetComponent<PlayerController>().m_bInCrane = false;
            m_players[i].GetComponent<PlayerController>().m_bIsOut = false;
            m_players[i].GetComponent<PlayerController>().m_bPickedUp = false;
            m_players[i].GetComponent<PlayerController>().m_bWeaponHit = false;
        }
    }
}