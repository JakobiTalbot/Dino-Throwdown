﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectPlay : MonoBehaviour
{
    // reference to the play button
    public UnityEngine.UI.Button m_playButton;

    private void Awake()
    {
        SelectButton();
    }

    // selects the play button
    public void SelectButton()
    {
        m_playButton.Select();
    }

    // quits the game
    public void ExitGame()
    {
        Application.Quit();
    }
}