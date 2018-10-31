using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    // the amount of ready players
    [HideInInspector]
    public int m_cReadyPlayers = 0;

    // the level that needs to be loaded
    private int m_nLevelNumber = 0;

    private void Update()
    {
        // plays the game when everyone is ready
        if (m_cReadyPlayers == 4)
        {
            SceneManager.LoadScene(m_nLevelNumber);
        }
    }

    // loads claw chaos when the players are ready
    public void LoadClawChaos()
    {
        m_nLevelNumber = 1;
    }
    // loads wrecking bowl when the players are ready
    public void LoadWreckingBowl()
    {
        m_nLevelNumber = 2;
    }
}