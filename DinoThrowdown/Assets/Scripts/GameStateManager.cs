using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public int m_nRoundsToWin = 3;
    public GameObject[] m_players;
    public GameObject m_confetti;
    // delay before round starts
    public float m_fStartDelay = 3.0f;
    // delay before round ends
    public float m_fEndDelay = 3.0f;

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
    private int m_nRoundNumber = 0;
    private bool m_bPlayerWon = false;
    // used to wait before the round starts
    private WaitForSeconds m_startWait;
    // used to wait before the round ends
    private WaitForSeconds m_endWait;

	// Use this for initialization
	void Awake()
    {
        m_cranes = GameObject.FindGameObjectsWithTag("Crane");
        m_claws = GameObject.FindGameObjectsWithTag("Claw");
        m_playerOriginalPositions = new Vector3[m_players.Length];
        m_playerOriginalRotations = new Quaternion[m_players.Length];
        m_craneOriginalPositions = new Vector3[m_cranes.Length];
        m_craneOriginalRotations = new Quaternion[m_cranes.Length];
        m_clawOriginalPositions = new Vector3[m_claws.Length];
        m_clawOriginalRotations = new Quaternion[m_claws.Length];
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

    private void Start()
    {
        // sets the wait times for the start and end
        m_startWait = new WaitForSeconds(m_fStartDelay);
        m_endWait = new WaitForSeconds(m_fEndDelay);

        // starts the game loop routine
        StartCoroutine(GameLoop());
    }

    // loops through the game
    private IEnumerator GameLoop()
    {
        // starts the start round coroutine
        yield return StartCoroutine(RoundStarting());
        // starts the playing round coroutine
        yield return StartCoroutine(RoundPlaying());
        // starts the end round coroutine
        yield return StartCoroutine(RoundEnding());

        // checks if there is a game winner
        if (m_bPlayerWon)
        {
            // reloads the scene
            SceneManager.LoadScene("Main");
        }
        else
        {
            // starts the game loop coroutine
            StartCoroutine(GameLoop());
        }
    }

    // sets up the round
    private IEnumerator RoundStarting()
    {
        // resets the round
        NewRound();
        // increment round number
        ++m_nRoundNumber;
        m_canvas.GetComponent<UI>().RoundText(m_nRoundNumber);
        DisableControls();

        // waits for the start delay
        yield return m_startWait;
    }

    // runs the round
    private IEnumerator RoundPlaying()
    {
        // enables the controls
        EnableControls();
        m_canvas.GetComponent<UI>().DisableText();

        // loops while there is not one player left
        while (m_playersRemaining.Count != 1)
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

            // waits a frame
            yield return null;
        }
    }

    // ends the round
    private IEnumerator RoundEnding()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            // find which player it was
            if (m_playersRemaining[0] == m_players[i])
            {
                // increment rounds won
                ++m_nRoundsWon[i];
                m_canvas.GetComponent<UI>().EnableRoundImage(m_players[i]);

                // check if they won the amount of rounds needed to win game
                if (m_nRoundsWon[i] == m_nRoundsToWin)
                {
                    m_confetti.SetActive(true);
                    m_bPlayerWon = true;
                    m_canvas.GetComponent<UI>().EnablePlayerWon(m_players[i]);
                }
            }
        }

        // waits for the end delay
        yield return m_endWait;
    }

    private void NewRound()
    {
        m_playersRemaining.Clear();
        for (int i = 0; i < m_players.Length; ++i)
        {
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
            m_players[i].GetComponent<PlayerController>().m_weaponSize.bFlag = false;
            m_players[i].GetComponent<PlayerController>().m_weaponSize.fTimer = 0.0f;
            m_players[i].GetComponent<PlayerController>().StopAttack();
            // reset player status
            m_players[i].GetComponent<PlayerController>().m_bInCrane = false;
            m_players[i].GetComponent<PlayerController>().m_bIsOut = false;
            m_players[i].GetComponent<PlayerController>().m_bPickedUp = false;
            m_players[i].GetComponent<PlayerController>().m_bWeaponHit = false;
            // set claw to null
            m_players[i].GetComponent<PlayerController>().m_claw = null;
            // set kinematic to false
            m_players[i].GetComponent<Rigidbody>().isKinematic = false;
            // reset dash timer
            m_players[i].GetComponent<Dash>().ResetTimer();
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
            m_claws[i].GetComponent<Claw>().m_bHasItem = false;
            m_claws[i].GetComponent<Claw>().m_bItemDrop = false;
            m_claws[i].GetComponentInChildren<Light>().color = new Color(0, 0, 0, 0);
        }
    }

    // sets all players to kinematic
    private void DisableControls()
    {
        foreach (var player in m_players)
        {
            player.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    // sets all players to not kinematic
    private void EnableControls()
    {
        foreach (var player in m_players)
        {
            player.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}