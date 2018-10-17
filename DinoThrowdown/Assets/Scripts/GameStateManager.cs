using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public int m_nRoundsToWin = 3;
    public GameObject[] m_players;

    private GameObject[] m_cranes;
    private GameObject[] m_claws;
    private GameObject m_canvas;
    private List<GameObject> m_playersRemaining;
    private Vector3[] m_playerOriginalPositions;
    private Quaternion[] m_playerOriginalRotations;
    private Vector3[] m_craneOriginalPositions;
    private Quaternion[] m_craneOriginalRotations;
    private Vector3[] m_clawOriginalPositions;
    private Quaternion[] m_clawOriginalRotations;
    private UI m_UI;
    private float m_fVictoryWaitTime = 5.0f;
    private int[] m_nRoundsWon;
    private int m_nRoundNumber = 1;
    private bool m_bPlayerWon = false;

	// Use this for initialization
	void Awake()
    {
        m_playerOriginalPositions = new Vector3[m_players.Length];
        m_playerOriginalRotations = new Quaternion[m_players.Length];
        m_playersRemaining = new List<GameObject>();
        m_nRoundsWon = new int[m_players.Length];
        m_canvas = GameObject.FindGameObjectWithTag("Canvas");
        m_cranes = GameObject.FindGameObjectsWithTag("Crane");
        m_claws = GameObject.FindGameObjectsWithTag("Claw");

        for (int i = 0; i < m_players.Length; ++i)
        {
            // set each player's rounds won to 0
            m_nRoundsWon[i] = 0;

            // add each player to remaining players
            m_playersRemaining.Add(m_players[i]);

            // get original positions/rotations of each player
            m_playerOriginalPositions[i] = m_players[i].transform.position;
            m_playerOriginalRotations[i] = m_players[i].transform.rotation;
        }

        for (int i = 0; i < m_cranes.Length; ++i)
        {
            // get original crane position/rotation
            m_craneOriginalPositions[i] = m_cranes[i].transform.localPosition;
            m_craneOriginalRotations[i] = m_cranes[i].transform.localRotation;

            // get original claw position/rotation
            m_clawOriginalPositions[i] = m_claws[i].transform.localPosition;
            m_clawOriginalRotations[i] = m_claws[i].transform.localRotation;
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
            
            for (int i = 0; i < m_players.Length; ++i)
            {
                // add players back
                if (!m_players[i].GetComponent<PlayerController>().m_bIsOut
                    && !m_players[i].GetComponent<PlayerController>().m_bInCrane
                    && !m_playersRemaining.Contains(m_players[i]))
                {
                    m_playersRemaining.Add(m_players[i]);
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
            m_players[i].transform.position = m_playerOriginalPositions[i];
            // reset player rotation
            m_players[i].transform.rotation = m_playerOriginalRotations[i];
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

            m_players[i].GetComponent<PlayerController>().m_claw = null;
        }

        for (int i = 0; i < m_cranes.Length; ++i)
        {
            m_cranes[i].transform.localPosition = m_craneOriginalPositions[i];
            m_cranes[i].transform.localRotation = m_craneOriginalRotations[i];
            m_cranes[i].GetComponent<CraneManager>().m_bOccupied = false;
            m_cranes[i].GetComponent<CraneManager>().m_player = null;

            m_claws[i].transform.localPosition = m_clawOriginalPositions[i];
            m_claws[i].transform.localRotation = m_clawOriginalRotations[i];
            m_claws[i].GetComponent<Claw>().m_bDropped = false;
            m_claws[i].GetComponentInChildren<Light>().color = new Color(0, 0, 0, 0);
        }
    }
}