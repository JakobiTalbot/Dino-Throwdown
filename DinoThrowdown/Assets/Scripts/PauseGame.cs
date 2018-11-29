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

    // Use this for initialization
    void Awake()
    {
        // gets the components from the children
        m_screenFader = GetComponentInChildren<Image>();
        m_buttons = GetComponentsInChildren<Button>();
        m_pausedText = GetComponentInChildren<Text>();

        m_buttons[0].Select();

        if (OptionsManager.InstanceExists)
        {
            // import values from singleton
            m_masterSlider.value = OptionsManager.Instance.m_fMasterVolume;
            m_musicSlider.value = OptionsManager.Instance.m_fMusicVolume;
            m_sfxSlider.value = OptionsManager.Instance.m_fSFXVolume;
            m_vibrationToggle.isOn = OptionsManager.Instance.m_bVibration;
        }
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
        if (m_options.activeSelf && OptionsManager.InstanceExists)
        {
            // export values to singleton
            OptionsManager.Instance.m_fMasterVolume = m_masterSlider.value;
            OptionsManager.Instance.m_fMusicVolume = m_musicSlider.value;
            OptionsManager.Instance.m_fSFXVolume = m_sfxSlider.value;
            OptionsManager.Instance.m_bVibration = m_vibrationToggle.isOn;
        }
    }

    // when the resume button is pressed (not the start button on the controller)
    public void Resume()
    {
        Time.timeScale = 1.0f;
        ResetAlpha();
        gameObject.SetActive(false);
    }

    public void ToggleShowOptions()
    {
        // toggle displaying pause or options screen
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
        // toggle displaying options or controls screen
        m_controls.SetActive(!m_controls.activeSelf);
        m_options.SetActive(!m_options.activeSelf);

        // select button based on which screen is showing
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
        // reset time scale so it doesn't break the main menu
        Time.timeScale = 1.0f;
        if (OptionsManager.InstanceExists)
        {
            OptionsManager.Instance.m_iScene = 0;
        }
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