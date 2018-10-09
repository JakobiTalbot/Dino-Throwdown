using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateMenu : MonoBehaviour
{
    // reference to the menu buttons canvas
    public GameObject m_menuButtons;

    private void Start()
    {
        m_menuButtons.SetActive(true);
    }
}