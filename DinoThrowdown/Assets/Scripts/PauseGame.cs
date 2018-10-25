using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    // the speed at which the canvas fades in
    public float m_fFadeSpeed = 2.0f;
    // reference to options UI aspects
    public GameObject m_options;
    // reference to controls UI aspects
    public GameObject m_controls;
    // reference to the master volume slider
    public Slider m_masterSlider;
    // reference to the music volume slider
    public Slider m_musicSlider;
    // reference to the sfx volume slider
    public Slider m_sfxSlider;
    // reference to the vibration toggle
    public Toggle m_vibrationToggle;
    // reference to controls button
    public Button m_controlsButton;
    // reference to return button
    public Button m_optionsReturnButton;
    public Button m_controlsReturnButton;

    // reference to the screen fade image
    private Image m_screenFader;
    // reference to the paused text
    private Text m_pausedText;
    // reference to the buttons
    [HideInInspector]
    public Button[] m_buttons;
    // used to increment the alpha of the objects
    private float m_fAlpha = 0.0f;
    // reference to OptionsManager instance
    private OptionsManager m_optionsInstance = OptionsManager.Instance;
    // used to know if the options values have been imported from singleton
    private bool m_bValuesSet = false;


    // Use this for initialization
    void Awake()
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
        if (m_options.activeSelf)
        {
            // check if values have been imported from singleton
            if (!m_bValuesSet)
            {
                // import values from singleton
                m_masterSlider.value = m_optionsInstance.m_fMasterVolume;
                m_musicSlider.value = m_optionsInstance.m_fMusicVolume;
                m_sfxSlider.value = m_optionsInstance.m_fSFXVolume;
                m_vibrationToggle.isOn = m_optionsInstance.m_bVibration;
                m_bValuesSet = true;
            }

            // export values to singleton
            m_optionsInstance.m_fMasterVolume = m_masterSlider.value;
            m_optionsInstance.m_fMusicVolume = m_musicSlider.value;
            m_optionsInstance.m_fSFXVolume = m_sfxSlider.value;
            m_optionsInstance.m_bVibration = m_vibrationToggle.isOn;
        }
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        ResetAlpha();
        gameObject.SetActive(false);
    }

    public void ToggleShowOptions()
    {
        m_options.SetActive(!m_options.activeSelf);
        m_pausedText.gameObject.SetActive(!m_pausedText.isActiveAndEnabled);
        foreach (var button in m_buttons)
        {
            button.gameObject.SetActive(!button.isActiveAndEnabled);
        }

        if (m_options.activeSelf)
        {
            m_masterSlider.Select();
        }
        else
        {
            m_buttons[0].Select();
        }
    }

    public void ToggleShowControls()
    {
        m_controls.SetActive(!m_controls.activeSelf);
        m_options.SetActive(!m_options.activeSelf);

        if (m_controls.activeSelf)
        {
            m_controlsReturnButton.Select();
        }
        else
        {
            m_masterSlider.Select();
        }
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