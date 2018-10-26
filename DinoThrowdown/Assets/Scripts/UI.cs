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
                // sets the colour of the text based on the colour of the player
                m_knockbackTexts[i].GetComponent<Text>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                // Get percentage knockback (without decimal places)
                int value = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
                m_knockbackTexts[i].GetComponent<Text>().text = value.ToString() + "%";

                // sets the colour of the image based on the colour of the player
                switch (i)
                {
                    case 0:
                        foreach (GameObject image in m_p1RoundImages)
                        {
                            image.GetComponent<Image>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                    case 1:
                        foreach (GameObject image in m_p2RoundImages)
                        {
                            image.GetComponent<Image>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                    case 2:
                        foreach (GameObject image in m_p3RoundImages)
                        {
                            image.GetComponent<Image>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                    case 3:
                        foreach (GameObject image in m_p4RoundImages)
                        {
                            image.GetComponent<Image>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                }

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

    // sets the round image and text based on the player
    public void EnableRoundImage(GameObject player)
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (m_players[i] == player)
            {
                GameObject[] playerRoundImages = null;
                m_roundWinText.SetActive(true);

                // sets the text based on the player
                if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 3 (Instance)")
                {
                    m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "BLUE";
                    m_roundWinText.GetComponent<Text>().color = new Color(0.0f, 0.5f, 1.0f);
                }
                else if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 1 (Instance)")
                {
                    m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "RED";
                    m_roundWinText.GetComponent<Text>().color = Color.red;
                }
                else if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 2 (Instance)")
                {
                    m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "GREEN";
                    m_roundWinText.GetComponent<Text>().color = Color.green;
                }
                else if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 4 (Instance)")
                {
                    m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "YELLOW";
                    m_roundWinText.GetComponent<Text>().color = Color.yellow;
                }

                switch (i)
                {
                    case 0:
                        playerRoundImages = m_p1RoundImages;
                        break;
                    case 1:
                        playerRoundImages = m_p2RoundImages;
                        break;
                    case 2:
                        playerRoundImages = m_p3RoundImages;
                        break;
                    case 3:
                        playerRoundImages = m_p4RoundImages;
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

    // sets the win text based on the player
    public void EnablePlayerWon(GameObject player)
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (m_players[i] == player)
            {
                m_winText.SetActive(true);

                // sets the text based on the player colour
                if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 3 (Instance)")
                {
                    m_winText.GetComponent<Text>().text = "BLUE WINS!";
                    m_winText.GetComponent<Text>().color = new Color(0.0f, 0.5f, 1.0f);
                }
                else if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 1 (Instance)")
                {
                    m_winText.GetComponent<Text>().text = "RED WINS!";
                    m_winText.GetComponent<Text>().color = Color.red;
                }
                else if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 2 (Instance)")
                {
                    m_winText.GetComponent<Text>().text = "GREEN WINS!";
                    m_winText.GetComponent<Text>().color = Color.green;
                }
                else if (m_players[i].GetComponent<MeshRenderer>().material.name == "DeleteTest 4 (Instance)")
                {
                    m_winText.GetComponent<Text>().text = "YELLOW WINS!";
                    m_winText.GetComponent<Text>().color = Color.yellow;
                }

                return;
            }
        }
    }

    // sets the round text based on the round number
    public void RoundText(int cRoundNumber)
    {
        m_roundText.SetActive(true);
        m_roundText.GetComponent<Text>().text = "ROUND " + cRoundNumber.ToString();
        m_roundWinText.SetActive(false);
    }
    // turns off the round text
    public void DisableText()
    {
        m_roundText.SetActive(false);
        m_roundWinText.SetActive(false);
    }
}