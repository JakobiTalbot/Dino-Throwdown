using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    // the speed at which the canvas fades in
    public float m_fFadeSpeed = 2.0f;

    // reference to the screen fade image
    private Image m_screenFader;
    // reference to the paused text
    private Text m_pausedText;
    // reference to the buttons
    [HideInInspector]
    public Button[] m_buttons;
    // used to increment the alpha of the objects
    private float m_fAlpha = 0.0f;

    // Use this for initialization
    void Start()
    {
        // gets the components from the children
        m_screenFader = GetComponentInChildren<Image>();
        m_buttons = GetComponentsInChildren<Button>();
        m_pausedText = GetComponentInChildren<Text>();

        m_buttons[0].Select();
    }

    // Update is called once per frame
    void Update()
    {
        // checks if the image is half visible
        if (m_screenFader.color.a < 0.49f)
        {
            // increments the alpha
            m_fAlpha += 0.0167f * m_fFadeSpeed;
            // ensures that the alpha is between 0 and 0.5
            Mathf.Clamp(m_fAlpha, 0.0f, 1.0f);

            // sets the components colour based on the new alpha
            m_screenFader.color = new Color(1.0f, 1.0f, 1.0f, m_fAlpha / 2.0f);
            m_pausedText.color = new Color(0.2f, 0.2f, 0.2f, m_fAlpha);

            // iterates through each button
            foreach (Button button in m_buttons)
            {
                button.image.color = new Color(1.0f, 1.0f, 1.0f, m_fAlpha);
                button.GetComponentInChildren<Text>().color = new Color(0.2f, 0.2f, 0.2f, m_fAlpha);
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        ResetAlpha();
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetAlpha()
    {
        Color color = m_screenFader.color;
        color.a = 0.0f;
        m_screenFader.color = color;

        m_fAlpha = 0.0f;

        // sets the components colour based on the new alpha
        m_screenFader.color = new Color(1.0f, 1.0f, 1.0f, m_fAlpha / 2.0f);
        m_pausedText.color = new Color(0.2f, 0.2f, 0.2f, m_fAlpha);

        // iterates through each button
        foreach (Button button in m_buttons)
        {
            button.image.color = new Color(1.0f, 1.0f, 1.0f, m_fAlpha);
            button.GetComponentInChildren<Text>().color = new Color(0.2f, 0.2f, 0.2f, m_fAlpha);
        }
    }
}