using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject[] m_knockbackTexts = new GameObject[4];
    public GameObject[] m_players = new GameObject[4];
    public GameObject[] m_knockbackBars = new GameObject[4];
    public GameObject[] m_p1RoundImages;
    public GameObject[] m_p2RoundImages;
    public GameObject[] m_p3RoundImages;
    public GameObject[] m_p4RoundImages;
    public GameObject m_winText;

    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            // Get percentage knockback (without decimal places)
            int value = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
            m_knockbackTexts[i].GetComponent<Text>().text = value.ToString() + "%";

            // Set bar size
            //m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta = new Vector2(value,
            //    m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.y);
        }
    }
	// Update is called once per frame
	void Update()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (m_players[i])
            {
                // Get percentage knockback (without decimal places)
                int value = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
                m_knockbackTexts[i].GetComponent<Text>().text = value.ToString() + "%";

                // Set bar size
                //m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta = new Vector2(value,
                //    m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.y);
            }
            else
            {
                // Set text for player to dead
                m_knockbackTexts[i].GetComponent<Text>().text = "DEAD";
                m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta = new Vector2(0,
                    m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.y);
            }
        }
    }

    public void EnableRoundImage(GameObject player)
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (m_players[i] == player)
            {
                GameObject[] playerRoundImages;
                if (i == 0)
                    playerRoundImages = m_p1RoundImages;
                else if (i == 1)
                    playerRoundImages = m_p2RoundImages;
                else if (i == 2)
                    playerRoundImages = m_p3RoundImages;
                else
                    playerRoundImages = m_p4RoundImages;

                for (int j = 0; j < GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameStateManager>().m_nRoundsToWin; ++j)
                {
                    if (!playerRoundImages[j].activeSelf)
                    {
                        playerRoundImages[i].SetActive(true);
                        return;
                    }
                }
            }
        }
    }

    public void EnablePlayerWon(GameObject player)
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (m_players[i] == player)
            {
                m_winText.GetComponent<Text>().text = "PLAYER " + i + " WINS!";
            }
        }
    }
}