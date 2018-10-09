using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    // reference to the menu buttons canvas
    public GameObject m_menuButtons;

    private void Update()
    {
        // checks if the cancel button is pressed
        if (Input.GetAxis("Cancel") != 0.0f)
        {
            // swaps the activeness of the canvases
            m_menuButtons.SetActive(true);
            m_menuButtons.GetComponent<SelectPlay>().SelectButton();
            gameObject.SetActive(false);
        }
    }
}