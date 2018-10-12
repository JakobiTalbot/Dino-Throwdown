using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // loads the game
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    // quits the game
    public void ExitGame()
    {
        Application.Quit();
    }
}