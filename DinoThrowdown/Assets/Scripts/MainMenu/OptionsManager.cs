using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : PersistantSingleton<OptionsManager>
{
    // reference to the master volume slider
    public Slider m_masterSlider;
    // reference to the music volume slider
    public Slider m_musicSlider;
    // reference to the sfx volume slider
    public Slider m_sfxSlider;
    // reference to the vibration toggle
    public Toggle m_vibrationToggle;
    // reference to the pickups toggle
    public Toggle m_pickupsToggle;
    // reference to the indicator toggle
    public Toggle m_indicatorToggle;
    // reference to the round amount dropdown
    public Dropdown m_roundDropdown;

    // the value of the master volume
    [HideInInspector]
    public float m_fMasterVolume = 100.0f;
    // the value of the music volume
    [HideInInspector]
    public float m_fMusicVolume = 100.0f;
    // the value of the sfx volume
    [HideInInspector]
    public float m_fSFXVolume = 100.0f;
    // determines if the vibration is on
    [HideInInspector]
    public bool m_bVibration = true;
    // determines if the pickups will spawn
    [HideInInspector]
    public bool m_bPickups = true;
    // determines if the indicator will be shown
    [HideInInspector]
    public bool m_bIndicator = true;
    // the value of the dropdown
    [HideInInspector]
    public int m_iRound = 1;

    private void Update()
    {
        // gets the values from the sliders
        m_fMasterVolume = m_masterSlider.value;
        m_fMusicVolume = m_musicSlider.value;
        m_fSFXVolume = m_sfxSlider.value;
        // gets the toggle status
        m_bVibration = m_vibrationToggle.isOn;
        m_bPickups = m_pickupsToggle.isOn;
        m_bIndicator = m_indicatorToggle.isOn;
        // gets the value from the dropdown
        m_iRound = m_roundDropdown.value;
    }
}