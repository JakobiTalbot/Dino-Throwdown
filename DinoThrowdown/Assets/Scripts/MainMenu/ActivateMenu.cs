using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateMenu : MonoBehaviour
{
    // reference to the menu buttons canvas
    public GameObject m_menuButtons;
    // reference to the game setup canvas
    public GameObject m_gameSetup;
    // reference to the background music
    public AudioSource m_backgroundMusic;

    private float m_fOriginalVolume = 1.0f;

    private void Awake()
    {
        // gets the original background music volume
        m_fOriginalVolume = m_backgroundMusic.volume;
    }

    private void Start()
    {
        MenuButtons();
    }

    private void Update()
    {
        if (OptionsManager.InstanceExists)
        {
            // changes the volume based on the options
            m_backgroundMusic.volume = OptionsManager.Instance.m_fMusicVolume * OptionsManager.Instance.m_fMasterVolume * m_fOriginalVolume;
        }
        // ensures that the time scale is 1 in the menu
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    // activates the menu buttons
    public void MenuButtons()
    {
        m_menuButtons.SetActive(true);
        m_menuButtons.GetComponent<SelectPlay>().SelectButton();
    }
    // activates the game setup
    public void GameSetup()
    {
        m_gameSetup.SetActive(true);
        m_gameSetup.GetComponent<GameSetup>().SelectButton();
        m_backgroundMusic.gameObject.SetActive(true);
    }
}