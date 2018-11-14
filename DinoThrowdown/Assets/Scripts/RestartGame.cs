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
                    m_playerViewers[0].GetComponent<MeshFilter>().mesh = m_players[i].GetComponentsInChildren<MeshFilter>()[1].mesh;
                    m_playerViewers[0].GetComponent<MeshRenderer>().material = m_players[i].GetComponentsInChildren<MeshRenderer>()[1].material;
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
                        m_playerViewers[i].GetComponent<MeshFilter>().mesh = m_players[j].GetComponentsInChildren<MeshFilter>()[1].mesh;
                        m_playerViewers[i].GetComponent<MeshRenderer>().material = m_players[j].GetComponentsInChildren<MeshRenderer>()[1].material;
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
    // loads the main menu scene
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}