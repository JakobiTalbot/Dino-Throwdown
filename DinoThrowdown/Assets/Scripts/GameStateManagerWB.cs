using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManagerWB : MonoBehaviour
{
    public int m_nLives = 3;
    public GameObject[] m_players;
    public GameObject m_confetti;
    // delay before round starts
    public float m_fStartDelay = 3.0f;
    // delay before round ends
    public float m_fEndDelay = 3.0f;
    // delay before players respawn
    public float m_fRespawnDelay = 1.0f;
    // game over canvas
    public RestartGame m_gameOverCanvas;
    // collection of seats in the audience stands
    public GameObject[] m_seats = new GameObject[3];

    private GameObject m_canvas;
    private Vector3[] m_playerOriginalPositions;
    private Quaternion[] m_playerOriginalRotations;
    private bool m_bPlayerWon = false;
    // used to wait before the round starts
    private WaitForSeconds m_startWait;
    // used to wait before the round ends
    private WaitForSeconds m_endWait;
    // reference to background music
    private AudioSource m_backgroundMusic;
    // collection of the amount of remaining lives
    private int[] m_cPlayerLives;
    // amount of remaining players
    private int m_cRemaining;
    // collection of pick up managers used for delaying after respawn
    private PlayerController.PickupTimer[] m_respawnDelays;
    // used to check if a seat is occupied
    private bool[] m_bSeats = new bool[3];

	// Use this for initialization
	private void Awake()
    {
        m_playerOriginalPositions = new Vector3[m_players.Length];
        m_playerOriginalRotations = new Quaternion[m_players.Length];
        m_canvas = GameObject.FindGameObjectWithTag("Canvas");

        m_backgroundMusic = GetComponent<AudioSource>();
        if (OptionsManager.InstanceExists)
        {
            m_backgroundMusic.volume = OptionsManager.Instance.m_fMusicVolume * OptionsManager.Instance.m_fMasterVolume;
            switch (OptionsManager.Instance.m_iRound)
            {
                case 0:
                    m_nLives = 1;
                    break;
                case 1:
                    m_nLives = 3;
                    break;
                case 2:
                    m_nLives = 5;
                    break;
            }
        }

        m_cPlayerLives = new int[m_players.Length];
        m_cRemaining = m_players.Length;
        m_respawnDelays = new PlayerController.PickupTimer[m_players.Length];

        for (int i = 0; i < m_bSeats.Length; i++)
        {
            m_bSeats[i] = false;
        }

        if (CharacterManager.InstanceExists)
        {
            for (int i = 0; i < m_players.Length; i++)
            {
                m_players[i].GetComponent<MeshFilter>().mesh = CharacterManager.Instance.m_dinoTypes[i];
                m_players[i].GetComponent<MeshRenderer>().material = CharacterManager.Instance.m_colours[i];
            }
        }

        for (int i = 0; i < m_players.Length; ++i)
        {
            // get original positions/rotations of each player
            m_playerOriginalPositions[i] = new Vector3(m_players[i].transform.position.x, 25, m_players[i].transform.position.z);
            m_playerOriginalRotations[i] = m_players[i].transform.rotation;
            m_cPlayerLives[i] = m_nLives;
            m_respawnDelays[i].bFlag = false;
            m_respawnDelays[i].fTimer = m_fRespawnDelay;
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
            // sets the game up for the end
            DisableControls();
            foreach (var player in m_players)
            {
                player.GetComponent<PlayerController>().m_bIsOut = true;
            }
            m_gameOverCanvas.gameObject.SetActive(true);
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
        DisableControls();
        m_canvas.GetComponent<UI>().m_startText.SetActive(true);

        // waits for the start delay
        yield return m_startWait;
    }

    // runs the round
    private IEnumerator RoundPlaying()
    {
        // enables the controls
        EnableControls();
        m_canvas.GetComponent<UI>().m_startText.SetActive(false);

        // loops while there is not one player left
        while (m_cRemaining != 1)
        {
            for (int i = 0; i < m_players.Length; ++i)
            {
                // remove player from players remaining if they are out
                if (m_players[i].GetComponent<PlayerController>().m_bIsOut && m_cPlayerLives[i] != 0)
                {
                    --m_cPlayerLives[i];
                    m_canvas.GetComponent<UI>().DisableRoundImage(m_players[i]);
                    m_players[i].GetComponent<PlayerController>().m_bIsOut = false;
                    
                    if (m_cPlayerLives[i] == 0)
                    {
                        --m_cRemaining;
                        m_players[i].GetComponent<PlayerController>().m_bIsOut = true;
                        for (int j = 0; j < m_seats.Length; j++)
                        {
                            if (!m_bSeats[j])
                            {
                                m_players[i].transform.position = m_seats[j].transform.position;
                                m_players[i].transform.rotation = m_seats[j].transform.rotation;
                                m_bSeats[j] = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_players[i].transform.position = m_playerOriginalPositions[i];
                        m_players[i].transform.rotation = m_playerOriginalRotations[i];
                        m_respawnDelays[i].bFlag = true;
                    }

                    m_players[i].GetComponent<Rigidbody>().isKinematic = true;
                    // reset player velocities
                    m_players[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                    m_players[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }

                if (m_respawnDelays[i].bFlag)
                {
                    m_respawnDelays[i].fTimer -= Time.deltaTime;
                    if (m_respawnDelays[i].fTimer <= 0.0f)
                    {
                        m_respawnDelays[i].fTimer = m_fRespawnDelay;
                        m_respawnDelays[i].bFlag = false;
                        m_players[i].GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
            }

            // waits a frame
            yield return null;
        }
    }

    // ends the round
    private IEnumerator RoundEnding()
    {
        if (m_cRemaining == 1)
        {
            for (int i = 0; i < m_players.Length; i++)
            {
                if (m_cPlayerLives[i] != 0)
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