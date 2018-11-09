using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : MonoBehaviour
{
    // the amount of ready players
    [HideInInspector]
    public int m_cReadyPlayers = 0;

    private void Update()
    {
        // plays the game when everyone is ready
        if (m_cReadyPlayers == 4)
        {
            SceneManager.LoadScene(1);
        }
    }
}