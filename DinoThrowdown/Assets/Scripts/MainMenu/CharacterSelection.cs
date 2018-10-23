using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public int m_cPlayerNumber;

    private GamePadState m_gamePadState;
    private Selectable[] m_selectables;
    private int m_iCurrentSelectable = 0;

    private void Awake()
    {
        m_gamePadState = GamePad.GetState((PlayerIndex)(m_cPlayerNumber - 1));
        m_selectables = GetComponentsInChildren<Selectable>();
    }

    private void Update()
    {
        if (m_iCurrentSelectable == 0)
        {

        }
    }
}