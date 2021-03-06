﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public float m_fKnockbackBarFillSpeed = 0.5f;
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
    // collection of colours
    public Material[] m_colours;

    // stores original size of knockbackbars
    private Vector2 m_v2OriginalBarSizeDelta;
    // determines if the win text should fade
    private bool m_bFade = false;

    // Use this for initialization
    void Awake()
    {
        // get original bar size delta
        m_v2OriginalBarSizeDelta = m_knockbackBars[0].GetComponent<RectTransform>().sizeDelta;
        for (int i = 0; i < m_players.Length; ++i)
        {
            // get percentage knockback (without decimal places)
            int nKnockbackValue = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
            m_knockbackTexts[i].GetComponent<Text>().text = nKnockbackValue.ToString() + "%";
            // set bar size
            m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta =
                new Vector2(Mathf.Lerp(m_knockbackBars[i].GetComponentsInChildren<RectTransform>()[0].sizeDelta.x, 0.0f +  (nKnockbackValue / 100.0f * m_v2OriginalBarSizeDelta.x), m_fKnockbackBarFillSpeed), m_v2OriginalBarSizeDelta.y);

            // set black bar size
            if (m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.x > 1.0f)
                m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta = 
                    new Vector2(m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.x + 4.0f,
                    m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta.y);
            else
                m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta =
                    new Vector2(0.0f,
                    m_knockbackBars[i].GetComponentInParent<RectTransform>().sizeDelta.y);


            // set UV rect size
            Rect rect = m_knockbackBars[i].GetComponent<RawImage>().uvRect;
            rect.width = m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.x / m_v2OriginalBarSizeDelta.x;
            m_knockbackBars[i].GetComponent<RawImage>().uvRect = rect;

            // set portrait images
            if (CharacterManager.InstanceExists)
            {
                m_knockbackTexts[i].GetComponent<Text>().color = m_colours[CharacterManager.Instance.m_iDinoColours[m_players[i].GetComponent<PlayerController>().m_cPlayerNumber - 1]].color;
                m_knockbackTexts[i].GetComponents<Shadow>()[0].effectColor = m_colours[CharacterManager.Instance.m_iDinoColours[m_players[i].GetComponent<PlayerController>().m_cPlayerNumber - 1]].color;
                // reference to the character manager instance
                CharacterManager charManager = CharacterManager.Instance;

                if (!CharacterManager.Instance.m_bActivePlayers[i])
                {
                    m_knockbackTexts[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    m_playerPortraits[i].GetComponent<RawImage>().texture = m_portraits.rows[charManager.m_nDinoColourIndex[i]].row[charManager.m_nDinoTypeIndex[i]];
                }
            }
        }
    }

	// Update is called once per frame
	void Update()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            if (m_knockbackTexts[i].transform.parent.gameObject.activeSelf)
            {
                // Get percentage knockback (without decimal places)
                int nKnockbackValue = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
                m_knockbackTexts[i].GetComponent<Text>().text = nKnockbackValue.ToString() + "%";

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

                // set bar size
                m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta =
                    new Vector2(Mathf.Lerp(m_knockbackBars[i].GetComponentsInChildren<RectTransform>()[0].sizeDelta.x, 0.0f + (nKnockbackValue / 100.0f * m_v2OriginalBarSizeDelta.x), m_fKnockbackBarFillSpeed), m_v2OriginalBarSizeDelta.y);

                // set black bar size
                if (m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.x > 1.0f)
                    m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta =
                        new Vector2(Mathf.Lerp(m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta.x, m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.x + 4.0f, m_fKnockbackBarFillSpeed),
                        m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta.y);
                else
                    m_knockbackBars[i].GetComponentsInParent<RectTransform>()[1].sizeDelta =
                        new Vector2(0.0f,
                        m_knockbackBars[i].GetComponentInParent<RectTransform>().sizeDelta.y);

                // set UV rect size
                Rect rect = m_knockbackBars[i].GetComponent<RawImage>().uvRect;
                rect.width = m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.x / m_v2OriginalBarSizeDelta.x;
                m_knockbackBars[i].GetComponent<RawImage>().uvRect = rect;
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
            if (!m_knockbackTexts[i].transform.parent.gameObject.activeSelf)
            {
                continue;
            }
            if (m_players[i] == player)
            {
                GameObject[] playerRoundImages = null;
                
                // enable round win text
                m_roundWinText.gameObject.SetActive(true);
                // set round win text colour based off colour of player who won round
                m_roundWinText.GetComponent<Text>().color = m_colours[CharacterManager.Instance.m_iDinoColours[m_players[i].GetComponent<PlayerController>().m_cPlayerNumber - 1]].color;

                // display text for the player who won
                m_roundWinText.GetComponent<Text>().text = "ROUND WIN\nPLAYER " + (i + 1);

                // find round images of winning player
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

                // activate round image
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
            if (!m_knockbackTexts[i].transform.parent.gameObject.activeSelf)
            {
                continue;
            }
            if (m_players[i] == player)
            {
                // activate win text
                m_winText.gameObject.SetActive(true);
                // set win text colour based on winner's colour
                m_winText.GetComponent<Text>().color = m_colours[CharacterManager.Instance.m_iDinoColours[m_players[i].GetComponent<PlayerController>().m_cPlayerNumber - 1]].color;

                // find which player won and display on screen
                m_winText.GetComponent<Text>().text = "PLAYER " + (i + 1) + " WINS!";

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
    // hides all the UI
    public void DisableUI()
    {
        for (int i = 0; i < m_knockbackTexts.Length; i++)
        {
            m_knockbackTexts[i].transform.parent.gameObject.SetActive(false);
        }
    }
}