using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPlay : MonoBehaviour
{
    // reference to the play button
    public UnityEngine.UI.Button m_playButton;

    private void Awake()
    {
        SelectButton();
    }

    public void SelectButton()
    {
        // selects the play button
        m_playButton.Select();
    }
}