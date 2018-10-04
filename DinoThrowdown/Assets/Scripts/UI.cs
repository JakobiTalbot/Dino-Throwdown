using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject[] m_knockbackTexts = new GameObject[4];
    public GameObject[] m_players = new GameObject[4];
    public GameObject[] m_knockbackBars = new GameObject[4];

    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < m_players.Length; ++i)
        {
            // Get percentage knockback (without decimal places)
            int value = (int)m_players[i].GetComponent<Knockback>().GetKnockback();
            m_knockbackTexts[i].GetComponent<Text>().text = value.ToString() + "%";

            // Set bar size
            m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta = new Vector2(value,
                m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.y);
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
                m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta = new Vector2(value,
                    m_knockbackBars[i].GetComponent<RectTransform>().sizeDelta.y);
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
}