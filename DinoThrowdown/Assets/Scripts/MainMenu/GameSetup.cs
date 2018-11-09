using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour {

    // reference to the play button
    public UnityEngine.UI.Button m_continueButton;

    private void Awake()
    {
        SelectButton();
    }

    // selects the play button
    public void SelectButton()
    {
        m_continueButton.Select();
    }
}