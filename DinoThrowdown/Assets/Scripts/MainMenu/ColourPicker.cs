using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPicker : MonoBehaviour
{
    // collection of references to the X over colours
    public GameObject[] m_colours;
    // collection of references to the other colour pickers
    public GameObject[] m_pickers;

    // checks if the colour is available
    public bool IsAvailable(int iColour)
    {
        if (m_colours[iColour].activeSelf)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // sets the availability of the specified colour
    public void SetSelfAvailability(bool bAvailability, int iColour)
    {
        m_colours[iColour].SetActive(!bAvailability);
    }

    // sets the availability of the specified colour for the other colour pickers
    public void SetOtherAvailability(bool bAvailability, int iColour)
    {
        foreach (GameObject picker in m_pickers)
        {
            picker.GetComponentInChildren<ColourPicker>().SetSelfAvailability(bAvailability, iColour);
        }
    }
}