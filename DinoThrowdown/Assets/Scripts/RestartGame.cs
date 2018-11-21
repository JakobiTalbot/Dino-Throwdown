using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    // reference to the win text
    public Text m_winText;
    // references to the players
    public PlayerController[] m_players;
    // references to the player viewers
    public GameObject[] m_playerViewers;

    // reference to the game over animator
    private Animator m_gameOverAnimation;

    private void Awake()
    {
        m_gameOverAnimation = GetComponent<Animator>();
    }

    private void Update()
    {
        // checks if all the player viewers are active
        if (m_playerViewers[0].activeSelf && m_playerViewers[1].activeSelf && m_playerViewers[2].activeSelf && m_playerViewers[3].activeSelf)
        {
            // stores the index of the winning player
            bool[] bAdded = new bool[4];

            // finds and stores the winning player
            for (int i = 0; i < m_players.Length; i++)
            {
                if (m_players[i].m_bWinner)
                {
                    for (int j = 0; j < m_players[i].m_dinos.Length; j++)
                    {
                        if (m_players[i].m_dinos[j].activeSelf)
                        {
                            m_players[i].m_dinos[j].transform.position = m_playerViewers[0].transform.position;
                            m_players[i].m_dinos[j].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                            break;
                        }
                    }
                    bAdded[i] = true;
                    break;
                }
            }

            // adds the other players to the player viewers
            for (int i = 1; i < m_playerViewers.Length; i++)
            {
                for (int j = 0; j < m_players.Length; j++)
                {
                    if (!bAdded[j])
                    {
                        for (int k = 0; k < m_players[j].m_dinos.Length; k++)
                        {
                            if (m_players[j].m_dinos[k].activeSelf)
                            {
                                m_players[j].m_dinos[k].transform.position = m_playerViewers[i].transform.position;
                                m_players[j].m_dinos[k].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                                break;
                            }
                        }
                        bAdded[j] = true;
                        break;
                    }
                }
            }
        }
    }

    // plays the game over animation
    public void PlayAnimation()
    {
        m_gameOverAnimation.SetTrigger("bGameOver");
    }

    // loads the main scene
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // selects the play button
    public void SelectPlay()
    {
        GetComponentInChildren<Button>().Select();
    }
    // loads the main menu scene
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}