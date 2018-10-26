using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    // the speed at which the canvas fades in
    public float m_fFadeSpeed = 2.0f;
    // reference to the text to be faded out
    public Text m_winText;

    // reference to the screen fade image
    private Image m_screenFader;
    // reference to the game over text
    private Text m_gameOverText;
    // reference to the buttons
    private Button[] m_buttons;
    // used to increment the alpha of the objects
    private float m_fAlpha = 0.0f;

    private void Start()
    {
        // gets the components from the children
        m_screenFader = GetComponentInChildren<Image>();
        m_gameOverText = GetComponentInChildren<Text>();
        m_buttons = GetComponentsInChildren<Button>();
        // selects the first button
        m_buttons[0].Select();
        // sets the text and colour of the game over text to the win text
        m_gameOverText.text = m_winText.text;
        m_gameOverText.color = m_winText.color;
    }

    private void Update()
    {
        // checks if the image is half visible
        if (m_screenFader.color.a < 0.49f)
        {
            // increments the alpha
            m_fAlpha += Time.deltaTime * m_fFadeSpeed;
            // ensures that the alpha is between 0 and 0.5
            Mathf.Clamp(m_fAlpha, 0.0f, 1.0f);

            // sets the components colour based on the new alpha
            m_screenFader.color = new Color(1.0f, 1.0f, 1.0f, m_fAlpha / 2.0f);
            m_gameOverText.color = new Color(m_gameOverText.color.r, m_gameOverText.color.g, m_gameOverText.color.b, m_fAlpha);

            // iterates through each button
            foreach (Button button in m_buttons)
            {
                button.image.color = new Color(1.0f, 1.0f, 1.0f, m_fAlpha);
                button.GetComponentInChildren<Text>().color = new Color(0.2f, 0.2f, 0.2f, m_fAlpha);
            }

            // fades out the win text
            m_winText.color = new Color(m_winText.color.r, m_winText.color.g, m_winText.color.b, 1.0f - m_fAlpha);
        }
    }

    // loads the main scene
    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }
    // loads the main menu scene
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}