using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewer : MonoBehaviour
{
    // collection of references to the 4 dino types
    public Mesh[] m_dinoTypes;
    // collections of references to the 16 dino colours
    public ArrayLayout m_dinoColours;

    // reference to the character selection script
    private CharacterSelection m_characterSelection;

    private void Start()
    {
        m_characterSelection = GetComponentInParent<CharacterSelection>();
    }

    private void Update()
    {
        // updates the display of the dino based on the users selected dino
        GetComponent<MeshFilter>().mesh = m_dinoTypes[m_characterSelection.m_iHoverpodType];
        GetComponent<MeshRenderer>().material = m_dinoColours.rows[m_characterSelection.m_iHoverpodColour].row[m_characterSelection.m_iHoverpodType];
        // stores the selected mesh and material in the character manager once the player has confirmed their character
        if (m_characterSelection.m_bConfirmed)
        {
            CharacterManager.Instance.m_iDinoTypes[m_characterSelection.m_cPlayerNumber - 1] = m_characterSelection.m_iHoverpodType;
            CharacterManager.Instance.m_dinoColours[m_characterSelection.m_cPlayerNumber - 1] = m_dinoColours.rows[m_characterSelection.m_iHoverpodColour].row[m_characterSelection.m_iHoverpodType];
            CharacterManager.Instance.m_iDinoColours[m_characterSelection.m_cPlayerNumber - 1] = m_characterSelection.m_iHoverpodColour;
        }
    }
}