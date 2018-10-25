using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using XInputDotNetPure;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    // the player reference number
    public int m_cPlayerNumber;
    // the delay between input from the controllers
    public float m_fInputDelay = 0.5f;
    // the delay for selecting different characters or weapons
    public float m_fHorizontalDelay = 0.4f;
    // collection of dino type names
    public string[] m_sDinoTypes;
    // collection of weapon type names
    public string[] m_sWeaponTypes;
    // collection of references to colour indicators
    public GameObject[] m_colourIndicators;
    // collection of references to indicators
    public GameObject[] m_indicators;
    // reference to the player viewer where the player will be displayed
    public GameObject m_playerViewer;
    // reference to the different meshes
    public Mesh[] m_dinoTypes;
    // collection of references to the different materials
    public Material[] m_colours;
    // reference to the confirm game object
    public GameObject m_confirm;

    // reference to the game pad
    private GamePadState m_gamePadState;
    // collection of references to selectables
    private Selectable[] m_selectables;
    // index of the current selectable
    private int m_iCurrentSelectable = 0;
    // used to time how long an input delay lasts for
    private float m_fInputTimer;
    // used to time how long a horizontal delay lasts for
    private float m_fHorizontalTimer;
    // the current dino type
    private int m_iDinoType;
    // the current weapon type
    private int m_iWeaponType;
    // the current colour
    private int m_iColour;
    // reference to the colour picker
    private ColourPicker m_colourPicker;
    // determines if the player has confirmed their player
    private bool m_bConfirmed = false;

    private void Awake()
    {
        m_gamePadState = GamePad.GetState((PlayerIndex)(m_cPlayerNumber - 1));
        m_selectables = GetComponentsInChildren<Selectable>();

        m_fInputTimer = m_fInputDelay;
        m_fHorizontalTimer = 0.0f;

        m_iDinoType = m_cPlayerNumber - 1;
        m_iWeaponType = m_cPlayerNumber - 1;
        m_iColour = m_cPlayerNumber - 1;

        m_colourPicker = GetComponentInChildren<ColourPicker>();
    }

    private void Update()
    {
        // get controller input
        m_gamePadState = GamePad.GetState((PlayerIndex)(m_cPlayerNumber - 1));

        m_fInputTimer -= Time.deltaTime;

        if (m_fHorizontalTimer >= 0.0f)
        {
            m_fHorizontalTimer -= Time.deltaTime;
        }

        if (m_iCurrentSelectable == 0 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            if (m_gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                m_iCurrentSelectable = 1;
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;

                if (m_iDinoType == 0)
                {
                    m_iDinoType = 3;
                }
                else
                {
                    m_iDinoType--;
                }

                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                m_iCurrentSelectable = 2;
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;

                if (m_iDinoType == 3)
                {
                    m_iDinoType = 0;
                }
                else
                {
                    m_iDinoType++;
                }

                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                m_iCurrentSelectable = 3;
                m_indicators[0].SetActive(false);
                m_indicators[1].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                switch (m_iColour)
                {
                    case 0:
                        m_iCurrentSelectable = 6;
                        break;
                    case 1:
                        m_iCurrentSelectable = 7;
                        break;
                    case 2:
                        m_iCurrentSelectable = 8;
                        break;
                    case 3:
                        m_iCurrentSelectable = 9;
                        break;
                }
                m_indicators[0].SetActive(false);
                m_indicators[2].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
        }
        else if ((m_iCurrentSelectable == 1 || m_iCurrentSelectable == 2) && m_fHorizontalTimer < 0.0f && !m_bConfirmed)
        {
            m_iCurrentSelectable = 0;
            m_selectables[1].image.color = Color.white;
            m_selectables[2].image.color = Color.white;
        }
        else if (m_iCurrentSelectable == 3 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            if (m_gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                m_iCurrentSelectable = 4;
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;

                if (m_iWeaponType == 0)
                {
                    m_iWeaponType = 3;
                }
                else
                {
                    m_iWeaponType--;
                }

                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                m_iCurrentSelectable = 5;
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;

                if (m_iWeaponType == 3)
                {
                    m_iWeaponType = 0;
                }
                else
                {
                    m_iWeaponType++;
                }

                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                switch (m_iColour)
                {
                    case 0:
                        m_iCurrentSelectable = 6;
                        break;
                    case 1:
                        m_iCurrentSelectable = 7;
                        break;
                    case 2:
                        m_iCurrentSelectable = 8;
                        break;
                    case 3:
                        m_iCurrentSelectable = 9;
                        break;
                }
                m_indicators[1].SetActive(false);
                m_indicators[2].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                m_iCurrentSelectable = 0;
                m_indicators[1].SetActive(false);
                m_indicators[0].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
        }
        else if ((m_iCurrentSelectable == 4 || m_iCurrentSelectable == 5) && m_fHorizontalTimer < 0.0f && !m_bConfirmed)
        {
            m_iCurrentSelectable = 3;
            m_selectables[4].image.color = Color.white;
            m_selectables[5].image.color = Color.white;
        }
        else if (m_iCurrentSelectable == 6 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            if (m_gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                m_iCurrentSelectable = 9;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 3;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                m_iCurrentSelectable = 7;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 1;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                m_iCurrentSelectable = 0;
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                m_iCurrentSelectable = 3;
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
        }
        else if (m_iCurrentSelectable == 7 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            if (m_gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                m_iCurrentSelectable = 6;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 0;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                m_iCurrentSelectable = 8;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 2;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                m_iCurrentSelectable = 0;
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                m_iCurrentSelectable = 3;
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
        }
        else if (m_iCurrentSelectable == 8 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            if (m_gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                m_iCurrentSelectable = 7;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 1;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                m_iCurrentSelectable = 9;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 3;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                m_iCurrentSelectable = 0;
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                m_iCurrentSelectable = 3;
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
        }
        else if (m_iCurrentSelectable == 9 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            if (m_gamePadState.ThumbSticks.Left.X < 0.0f)
            {
                m_iCurrentSelectable = 8;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 2;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.X > 0.0f)
            {
                m_iCurrentSelectable = 6;
                m_colourIndicators[m_iColour].SetActive(false);
                m_iColour = 0;
                m_colourIndicators[m_iColour].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y < 0.0f)
            {
                m_iCurrentSelectable = 0;
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
            else if (m_gamePadState.ThumbSticks.Left.Y > 0.0f)
            {
                m_iCurrentSelectable = 3;
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);

                m_fInputTimer = m_fInputDelay;
            }
        }

        if ((m_gamePadState.Buttons.A == ButtonState.Pressed || Input.GetAxis("Fire" + m_cPlayerNumber) != 0.0f) && m_colourPicker.IsAvailable(m_iColour) && !m_bConfirmed && m_fInputTimer < 0.0f)
        {
            m_colourPicker.SetOtherAvailability(false, m_iColour);
            m_bConfirmed = true;
            m_confirm.SetActive(true);

            CharacterManager.Instance.m_dinoTypes[m_cPlayerNumber - 1] = m_dinoTypes[m_iDinoType];
            CharacterManager.Instance.m_colours[m_cPlayerNumber - 1] = m_colours[m_iColour];

            GetComponentInParent<PlayGame>().m_cReadyPlayers++;

            m_fInputTimer = m_fInputDelay;
        }

        if (m_gamePadState.Buttons.B == ButtonState.Pressed && m_bConfirmed && m_fInputTimer < 0.0f)
        {
            m_colourPicker.SetOtherAvailability(true, m_iColour);
            m_bConfirmed = false;
            m_confirm.SetActive(false);

            GetComponentInParent<PlayGame>().m_cReadyPlayers--;
            
            m_fInputTimer = m_fInputDelay;
        }
        else if (m_gamePadState.Buttons.B == ButtonState.Pressed && m_fInputTimer < 0.0f)
        {
            transform.parent.GetComponentInParent<ActivateMenu>().MenuButtons();
            transform.parent.gameObject.SetActive(false);
            m_fInputTimer = m_fInputDelay;
        }

        m_selectables[0].GetComponentInChildren<Text>().text = m_sDinoTypes[m_iDinoType];
        m_selectables[3].GetComponentInChildren<Text>().text = m_sWeaponTypes[m_iWeaponType];

        m_playerViewer.GetComponent<MeshFilter>().mesh = m_dinoTypes[m_iDinoType];
        m_playerViewer.GetComponent<MeshRenderer>().material = m_colours[m_iColour];
    }
}