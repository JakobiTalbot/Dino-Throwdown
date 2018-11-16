using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject m_gameManager;
    public GameObject[] m_knockbackTexts = new GameObject[4];
    public GameObject[] m_players = new GameObject[4];
    public GameObject[] m_knockbackBars = new GameObject[4];
    public GameObject[] m_p1RoundImages;
    public GameObject[] m_p2RoundImages;
    public GameObject[] m_p3RoundImages;
    public GameObject[] m_p4RoundImages;
    public GameObject[] m_playerPortraits;
    public TextureArrayLayout m_portraits;
    public Text m_winText;
    public Text m_roundWinText;
    public Text m_roundText;
    public Texture m_roundWinIcon;

    // determines if the win text should fade
    private bool m_bFade = false;

    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            // Get percentage knockback (without decimal places)
            int value = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
            m_knockbackTexts[i].GetComponent<Text>().text = value.ToString() + "%";

            // set portrait images
            if (CharacterManager.InstanceExists)
            {
                CharacterManager charManager = CharacterManager.Instance;
                for (int j = 0; j < m_playerPortraits.Length; ++j)
                {
                    m_playerPortraits[j].GetComponent<RawImage>().texture = m_portraits.rows[charManager.m_nDinoTypeIndex[j]].row[charManager.m_nDinoColourIndex[j]];
                }
            }
            
            // set round images to active based on how many rounds there are
            for (int j = 0; j < m_gameManager.GetComponent<GameStateManager>().m_nRoundsToWin; ++j)
            {
                m_p1RoundImages[j].SetActive(true);
                m_p2RoundImages[j].SetActive(true);
                m_p3RoundImages[j].SetActive(true);
                m_p4RoundImages[j].SetActive(true);
            }

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
                            image.GetComponent<RawImage>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                    case 1:
                        foreach (GameObject image in m_p2RoundImages)
                        {
                            image.GetComponent<RawImage>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                    case 2:
                        foreach (GameObject image in m_p3RoundImages)
                        {
                            image.GetComponent<RawImage>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
                        }
                        break;
                    case 3:
                        foreach (GameObject image in m_p4RoundImages)
                        {
                            image.GetComponent<RawImage>().color = m_players[i].GetComponent<MeshRenderer>().material.color;
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

        if (m_bFade)
        {
            m_winText.color = new Color(m_winText.color.r, m_winText.color.g, m_winText.color.b, m_winText.color.a - Time.deltaTime);
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
                m_roundWinText.gameObject.SetActive(true);
                m_roundWinText.GetComponent<Text>().color = m_players[i].GetComponent<MeshRenderer>().material.color;

                // display text for the player who won
                switch (i)
                {
                    case 0:
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "PLAYER 1";
                        playerRoundImages = m_p1RoundImages;
                        break;
                    case 1:
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "PLAYER 2";
                        playerRoundImages = m_p2RoundImages;
                        break;
                    case 2:
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "PLAYER 3";
                        playerRoundImages = m_p3RoundImages;
                        break;
                    case 3:
                        m_roundWinText.GetComponent<Text>().text = "ROUND WIN" + "\n" + "PLAYER 4";
                        playerRoundImages = m_p4RoundImages;
                        break;
                }

                for (int j = 0; j < GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameStateManager>().m_nRoundsToWin; ++j)
                {
                    if (playerRoundImages[j].GetComponent<RawImage>().texture != m_roundWinIcon)
                    {
                        playerRoundImages[j].GetComponent<RawImage>().texture = m_roundWinIcon;
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
                m_winText.gameObject.SetActive(true);
                m_winText.GetComponent<Text>().color = m_players[i].GetComponent<MeshRenderer>().material.color;

                switch (i)
                {
                    case 0:
                        m_winText.GetComponent<Text>().text = "PLAYER 1 WINS!";
                        break;
                    case 1:
                        m_winText.GetComponent<Text>().text = "PLAYER 2 WINS!";
                        break;
                    case 2:
                        m_winText.GetComponent<Text>().text = "PLAYER 3 WINS!";
                        break;
                    case 3:
                        m_winText.GetComponent<Text>().text = "PLAYER 4 WINS!";
                        break;
                }

                return;
            }
        }
    }

    // sets the round text based on the round number
    public void RoundText(int cRoundNumber)
    {
        m_roundText.gameObject.SetActive(true);
        m_roundText.text = "ROUND " + cRoundNumber.ToString();
        m_roundWinText.gameObject.SetActive(false);
    }
    // turns off the round text
    public void DisableText()
    {
        m_roundText.gameObject.SetActive(false);
        m_roundWinText.gameObject.SetActive(false);
    }
    // fades out the win text
    public void FadeText()
    {
        m_bFade = true;
    }
}