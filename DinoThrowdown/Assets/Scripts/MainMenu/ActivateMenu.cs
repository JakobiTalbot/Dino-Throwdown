using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateMenu : MonoBehaviour
{
    // reference to the menu buttons canvas
    public GameObject m_menuButtons;

    // reference to the background music
    private AudioSource m_backgroundMusic;

    private void Start()
    {
        m_menuButtons.SetActive(true);
        m_backgroundMusic = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // changes the volume based on the options
        m_backgroundMusic.volume = OptionsManager.Instance.m_fMusicVolume * OptionsManager.Instance.m_fMasterVolume;
    }
}