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
    public GameObject m_roundWinText;
    public GameObject m_roundText;

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
                GameObject[] playerRoundImages = null;
                m_roundWinText.SetActive(true);

                switch (i)
                {
                    case 0:
                        playerRoundImages = m_p1RoundImages;
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "BLUE";
                        m_roundWinText.GetComponent<Text>().color = new Color(0.0f, 0.5f, 1.0f);
                        break;
                    case 1:
                        playerRoundImages = m_p2RoundImages;
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "RED";
                        m_roundWinText.GetComponent<Text>().color = Color.red;
                        break;
                    case 2:
                        playerRoundImages = m_p3RoundImages;
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "GREEN";
                        m_roundWinText.GetComponent<Text>().color = Color.green;
                        break;
                    case 3:
                        playerRoundImages = m_p4RoundImages;
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "YELLOW";
                        m_roundWinText.GetComponent<Text>().color = Color.yellow;
                        break;
                }

                for (int j = 0; j < GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameStateManager>().m_nRoundsToWin; ++j)
                {
                    if (!playerRoundImages[j].activeSelf)
                    {
                        playerRoundImages[j].SetActive(true);
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
                m_winText.SetActive(true);

                switch (i)
                {
                    case 0:
                        m_winText.GetComponent<Text>().text = "BLUE WINS!";
                        m_winText.GetComponent<Text>().color = new Color(0.0f, 0.5f, 1.0f);
                        break;
                    case 1:
                        m_winText.GetComponent<Text>().text = "RED WINS!";
                        m_winText.GetComponent<Text>().color = Color.red;
                        break;
                    case 2:
                        m_winText.GetComponent<Text>().text = "GREEN WINS!";
                        m_winText.GetComponent<Text>().color = Color.green;
                        break;
                    case 3:
                        m_winText.GetComponent<Text>().text = "YELLOW WINS!";
                        m_winText.GetComponent<Text>().color = Color.yellow;
                        break;
                }

                return;
            }
        }
    }

    public void RoundText(int cRoundNumber)
    {
        m_roundText.SetActive(true);
        m_roundText.GetComponent<Text>().text = "ROUND " + cRoundNumber.ToString();
        m_roundWinText.SetActive(false);
    }
    public void DisableText()
    {
        m_roundText.SetActive(false);
        m_roundWinText.SetActive(false);
    }
}