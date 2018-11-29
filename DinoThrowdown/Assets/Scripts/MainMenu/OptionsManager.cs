using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : PersistantSingleton<OptionsManager>
{
    // the value of the master volume
    [HideInInspector]
    public float m_fMasterVolume = 1.0f;
    // the value of the music volume
    [HideInInspector]
    public float m_fMusicVolume = 1.0f;
    // the value of the sfx volume
    [HideInInspector]
    public float m_fSFXVolume = 1.0f;
    // determines if the vibration is on
    [HideInInspector]
    public bool m_bVibration = true;
    // determines if the wrecking ball will swing
    [HideInInspector]
    public bool m_bWreckingBall = true;
    // determines if the bombs will be dropped in
    [HideInInspector]
    public bool m_bBombs = true;
    // determines if the pickups will spawn
    [HideInInspector]
    public bool m_bPickups = true;
    // determines if the indicator will be shown
    [HideInInspector]
    public bool m_bIndicator = true;
    // the value of the dropdown
    [HideInInspector]
    public int m_iRound = 1;

    // references to the sliders
    private Slider[] m_sliders;
    // references to the toggles
    private Toggle[] m_toggles;
    // reference to the round dropdown
    private Dropdown m_dropdown;

    protected override void Awake()
    {
        base.Awake();

        m_fMasterVolume /= 100.0f;
        m_fMusicVolume /= 100.0f;
        m_fSFXVolume /= 100.0f;
    }

    private void Update()
    {
        m_sliders = FindObjectsOfType<Slider>();
        m_toggles = FindObjectsOfType<Toggle>();
        m_dropdown = FindObjectOfType<Dropdown>();

        // gets the values from the sliders
        if (m_sliders != null && m_sliders.Length >= 3)
        {
            m_fMasterVolume = m_sliders[1].value;
            m_fMusicVolume = m_sliders[2].value;
            m_fSFXVolume = m_sliders[0].value;
        }
        // gets the toggle status
        if (m_toggles != null && m_toggles.Length >= 4)
        {
            m_bPickups = m_toggles[0].isOn;
            m_bIndicator = m_toggles[1].isOn;
            m_bBombs = m_toggles[2].isOn;
            m_bWreckingBall = m_toggles[3].isOn;
        }
        else if (m_toggles != null && m_toggles.Length >= 1)
        {
            m_bVibration = m_toggles[0].isOn;
        }
        // gets the value from the dropdown
        if(m_dropdown != null)
            m_iRound = m_dropdown.value;
    }
}