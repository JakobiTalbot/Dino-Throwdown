using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public int m_nRoundsToWin = 3;
    public GameObject[] m_players;
    public GameObject m_confetti;
    // delay before round starts
    public float m_fStartDelay = 3.0f;
    // delay before round ends
    public float m_fEndDelay = 3.0f;
    // game over canvas
    public RestartGame m_gameOverCanvas;
    // reference to bombdropper object
    public GameObject m_bombDropper;
    // stores reference to all powerup spawnpoints
    public GameObject[] m_powerupSpawnPoints;
    public GameObject m_pickupManager;

    [HideInInspector]
    public int m_nRoundNumber = 0;
    [HideInInspector]
    public bool m_bPlayerWon = false;

    private GameObject[] m_cranes;
    private GameObject[] m_claws;
    private GameObject m_canvas;
    private List<GameObject> m_playersRemaining;
    private Vector3[] m_playerOriginalPositions;
    private Quaternion[] m_playerOriginalRotations;
    private Vector3[] m_clawOriginalPositions;
    private Quaternion[] m_clawOriginalRotations;
    private int[] m_nRoundsWon;
    // used to wait before the round starts
    private WaitForSeconds m_startWait;
    // used to wait before the round ends
    private WaitForSeconds m_endWait;
    // reference to background music
    private AudioSource m_backgroundMusic;
    // reference to the wrecking ball
    private WreckingBall m_wreckingBall;

	// Use this for initialization
	void Awake()
    {
        m_cranes = GameObject.FindGameObjectsWithTag("Crane");
        m_claws = GameObject.FindGameObjectsWithTag("Claw");
        m_playerOriginalPositions = new Vector3[m_players.Length];
        m_playerOriginalRotations = new Quaternion[m_players.Length];
        m_clawOriginalPositions = new Vector3[m_claws.Length];
        m_clawOriginalRotations = new Quaternion[m_claws.Length];
        m_playersRemaining = new List<GameObject>();
        m_nRoundsWon = new int[m_players.Length];
        m_canvas = GameObject.FindGameObjectWithTag("Canvas");
        m_wreckingBall = GameObject.FindGameObjectWithTag("WreckingBall").GetComponent<WreckingBall>();

        m_backgroundMusic = GetComponent<AudioSource>();
        if (OptionsManager.InstanceExists)
        {
            m_backgroundMusic.volume = OptionsManager.Instance.m_fMusicVolume * OptionsManager.Instance.m_fMasterVolume;
            switch (OptionsManager.Instance.m_iRound)
            {
                case 0:
                    m_nRoundsToWin = 1;
                    break;
                case 1:
                    m_nRoundsToWin = 3;
                    break;
                case 2:
                    m_nRoundsToWin = 5;
                    break;
            }
        }

        if (CharacterManager.InstanceExists)
        {
            for (int i = 0; i < m_players.Length; i++)
            {
                if (CharacterManager.Instance.m_bActivePlayers[i])
                {
                    m_players[i].GetComponent<MeshFilter>().mesh = CharacterManager.Instance.m_hoverpodTypes[i];
                    m_players[i].GetComponent<MeshRenderer>().material = CharacterManager.Instance.m_hoverpodColours[i];
                }
                else
                {
                    m_players[i].SetActive(false);
                }
            }
        }

        for (int i = 0; i < m_players.Length; ++i)
        {
            if (!m_players[i].activeSelf)
            {
                continue;
            }

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
            // sets the game up for the end
            DisableControls();
            foreach (var player in m_players)
            {
                if (!player.activeSelf)
                {
                    continue;
                }

                player.GetComponent<PlayerController>().m_bIsOut = true;
                player.GetComponent<PlayerController>().m_konamiCamera.gameObject.SetActive(false);
            }
            m_canvas.GetComponent<UI>().FadeText();
            m_canvas.GetComponent<UI>().DisableUI();
            m_gameOverCanvas.PlayAnimation();
            m_gameOverCanvas.m_winText.text = m_canvas.GetComponent<UI>().m_winText.text;
            m_gameOverCanvas.m_winText.color = new Color(m_canvas.GetComponent<UI>().m_winText.color.r, m_canvas.GetComponent<UI>().m_winText.color.g, m_canvas.GetComponent<UI>().m_winText.color.b, 0.0f);
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
        // increment round number
        ++m_nRoundNumber;
        // resets the round
        NewRound();

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
                if (!m_players[i].activeSelf)
                {
                    continue;
                }

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
            if (!m_players[i].activeSelf)
            {
                continue;
            }

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
                    m_players[i].GetComponent<PlayerController>().m_bWinner = true;
                    m_canvas.GetComponent<UI>().EnablePlayerWon(m_players[i]);
                    m_canvas.GetComponent<UI>().DisableText();
                }
            }
        }

        for (int i = 0; i < m_claws.Length; i++)
        {
            // stops the flashing animation
            m_claws[i].GetComponent<Claw>().StopFlashing();
        }

        // waits for the end delay
        yield return m_endWait;
    }

    private void NewRound()
    {
        m_playersRemaining.Clear();
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (!m_players[i].activeSelf)
            {
                continue;
            }

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
            // set kinematic to false
            m_players[i].GetComponent<Rigidbody>().isKinematic = false;
            // reset powerups
            m_players[i].GetComponent<PlayerController>().m_cruiseControl.bFlag = false;
            m_players[i].GetComponent<PlayerController>().m_cruiseControl.fTimer = 0.0f;
            m_players[i].GetComponent<Knockback>().m_shield.bFlag = false;
            m_players[i].GetComponent<Knockback>().m_shield.fTimer = 0.0f;
            m_players[i].GetComponent<Knockback>().m_shieldSphere.SetActive(false);
            m_players[i].GetComponent<PlayerController>().m_weaponSize.bFlag = false;
            m_players[i].GetComponent<PlayerController>().m_weaponSize.fTimer = 0.0f;
            m_players[i].GetComponent<PlayerController>().m_weapon.transform.localScale = m_players[i].GetComponent<PlayerController>().m_v3BaseWeaponScale;
            m_players[i].GetComponent<PlayerController>().m_weapon.transform.localPosition = m_players[i].GetComponent<PlayerController>().m_v3BaseWeaponPosition;
            // reset player status
            m_players[i].GetComponent<PlayerController>().m_bInCrane = false;
            m_players[i].GetComponent<PlayerController>().m_bIsOut = false;
            m_players[i].GetComponent<PlayerController>().StopAttack();
            m_players[i].GetComponent<PlayerController>().m_cAttackAmount = 0;
            // set claw to null
            m_players[i].GetComponent<PlayerController>().m_claw = null;
            // reset dash timer
            m_players[i].GetComponent<Dash>().ResetTimer();
        }

        for (int i = 0; i < m_cranes.Length; ++i)
        {
            m_cranes[i].GetComponent<CraneManager>().m_bOccupied = false;
            m_cranes[i].GetComponent<CraneManager>().m_player = null;

            m_claws[i].transform.localPosition = m_clawOriginalPositions[i];
            m_claws[i].transform.localRotation = m_clawOriginalRotations[i];
            m_claws[i].GetComponent<Claw>().m_bDropped = false;
            m_claws[i].GetComponent<Claw>().m_bHasItem = false;
            GameObject clawBomb = m_claws[i].GetComponent<Claw>().m_item;
            Destroy(clawBomb);
            m_claws[i].GetComponent<Claw>().m_item = null;
            m_claws[i].GetComponent<Claw>().m_bHasPlayer = false;
            m_claws[i].GetComponent<Claw>().m_pickedUpPlayer = null;
            m_claws[i].GetComponentInChildren<LineRenderer>().SetPosition(1, Vector3.zero);
        }

        // resets the wrecking ball
        m_wreckingBall.ResetBall();

        BombDropper dropperScript = m_bombDropper.GetComponent<BombDropper>();
        // reset bombdropper
        dropperScript.m_blocks.Clear();
        dropperScript.AddBlocks();
        dropperScript.ResetBlocks();
        dropperScript.m_bGettingBomb = false;
        dropperScript.m_bHasBomb = false;
        dropperScript.ResetTimer();
        dropperScript.transform.position = new Vector3(0.0f, dropperScript.m_fStartYPos, 0.0f);
        // destroy bomb if there is one
        if (dropperScript.m_currentBomb)
            Destroy(dropperScript.m_currentBomb);

        m_bombDropper.GetComponent<BombDropper>().m_fTimeUntilBombsDrop = m_bombDropper.GetComponent<BombDropper>().m_fSecondsUntilBombsStartDropping;
        m_bombDropper.GetComponent<BombDropper>().m_fDropTimer = m_bombDropper.GetComponent<BombDropper>().m_fDropInterval;

        // reset all pickup spawnpoints
        foreach (var spawnPoint in m_powerupSpawnPoints)
        {
            spawnPoint.GetComponent<SpawnPoint>().m_bHasPickup = false;
        }

        // destroy all pickups in scene
        foreach (var pickup in m_pickupManager.GetComponent<PickupManager>().m_spawnedPickups)
        {
            Destroy(pickup);
        }

        // clear list of pickups
        m_pickupManager.GetComponent<PickupManager>().m_spawnedPickups.Clear();

        // disable all bombs in scene
        foreach (var bomb in GameObject.FindGameObjectsWithTag("Bomb"))
        {
            Destroy(bomb);
        }
    }

    // sets all players to kinematic
    private void DisableControls()
    {
        foreach (var player in m_players)
        {
            if (!player.activeSelf)
            {
                continue;
            }

            player.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    // sets all players to not kinematic
    private void EnableControls()
    {
        foreach (var player in m_players)
        {
            if (!player.activeSelf)
            {
                continue;
            }

            player.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}