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
    // the amount of input required to do something
    public float m_fInputPrerequisite = 0.1f;
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
    public Mesh[] m_hoverpodTypes;
    // collections of references to the different materials
    public ArrayLayout m_hoverpodColours;
    // reference to the confirm game object
    public GameObject m_confirmCanvas;

    // the current dino type
    [HideInInspector]
    public int m_iHoverpodType;
    // the current colour
    [HideInInspector]
    public int m_iHoverpodColour;
    // determines if the player has confirmed their player
    [HideInInspector]
    public bool m_bConfirmed = false;

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
    // the current weapon type
    private int m_iWeaponType;
    // reference to the colour picker
    private ColourPicker m_colourPicker;

    private void Awake()
    {
        // gets all the selectable objects
        m_selectables = GetComponentsInChildren<Selectable>();

        // ensures that there is a delay before input is given
        m_fInputTimer = m_fInputDelay;
        m_fHorizontalTimer = 0.0f;

        // sets the initial dino type, weapon type and colour based on the player number
        m_iHoverpodType = m_cPlayerNumber - 1;
        m_iWeaponType = m_cPlayerNumber - 1;
        m_iHoverpodColour = m_cPlayerNumber - 1;

        m_colourPicker = GetComponentInChildren<ColourPicker>();
    }

    private void Update()
    {
        // get controller input
        m_gamePadState = GamePad.GetState((PlayerIndex)(m_cPlayerNumber - 1));
        // decrements the input timer
        if (m_fInputTimer >= 0.0f)
        {
            m_fInputTimer -= Time.deltaTime;
        }
        // decrements the timer for left and right button selection
        if (m_fHorizontalTimer >= 0.0f)
        {
            m_fHorizontalTimer -= Time.deltaTime;
        }

        // checks if the dino type button is selected, the input timer is finished and the player has not yet confirmed their character
        if (m_iCurrentSelectable == 0 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            // checks if a left input is given
            if (m_gamePadState.ThumbSticks.Left.X < -m_fInputPrerequisite || m_gamePadState.DPad.Left == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the left button
                m_iCurrentSelectable = 1;
                // highlights the selectable
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;
                // checks if the dino type needs to wrap around
                if (m_iHoverpodType == 0)
                {
                    m_iHoverpodType = 3;
                }
                else
                {
                    // decrements the dino type
                    m_iHoverpodType--;
                }
                // resets the timers
                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a right input is given
            else if (m_gamePadState.ThumbSticks.Left.X > m_fInputPrerequisite || m_gamePadState.DPad.Right == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the right button
                m_iCurrentSelectable = 2;
                // highlights the selectable
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;
                // checks if the dino type needs to wrap around
                if (m_iHoverpodType == 3)
                {
                    m_iHoverpodType = 0;
                }
                else
                {
                    // increments the dino type
                    m_iHoverpodType++;
                }
                // resets the timers
                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a down input is given
            else if (m_gamePadState.ThumbSticks.Left.Y < -m_fInputPrerequisite || m_gamePadState.DPad.Down == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the weapon type button
                m_iCurrentSelectable = 3;
                // moves the indicator down
                m_indicators[0].SetActive(false);
                m_indicators[1].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if an up input is given
            else if (m_gamePadState.ThumbSticks.Left.Y > m_fInputPrerequisite || m_gamePadState.DPad.Up == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object based on the current colour
                switch (m_iHoverpodColour)
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
                // moves the indicator down to the colours
                m_indicators[0].SetActive(false);
                m_indicators[2].SetActive(true);
                // resets the timers
                m_fInputTimer = m_fInputDelay;
            }
        }
        // checks if the left or right dino type button is selected, the delay is finished and the player has not yet confirmed
        else if ((m_iCurrentSelectable == 1 || m_iCurrentSelectable == 2) && m_fHorizontalTimer < 0.0f && !m_bConfirmed)
        {
            // sets the selected object to the dino type button
            m_iCurrentSelectable = 0;
            // sets the button colour back to normal
            m_selectables[1].image.color = Color.white;
            m_selectables[2].image.color = Color.white;
        }
        // checks if the weapon type button is selected, the input timer is finished and the player has not yet confirmed their character
        else if (m_iCurrentSelectable == 3 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            // checks if a left input is given
            if (m_gamePadState.ThumbSticks.Left.X < -m_fInputPrerequisite || m_gamePadState.DPad.Left == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the left button
                m_iCurrentSelectable = 4;
                // highlights the selectable
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;
                // checks if the weapon type needs to wrap around
                if (m_iWeaponType == 0)
                {
                    m_iWeaponType = 3;
                }
                else
                {
                    // decrements the weapon type
                    m_iWeaponType--;
                }
                // resets the timers
                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a right input is given
            else if (m_gamePadState.ThumbSticks.Left.X > m_fInputPrerequisite || m_gamePadState.DPad.Right == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the right button
                m_iCurrentSelectable = 5;
                // highlights the selectable
                m_selectables[m_iCurrentSelectable].image.color = Color.cyan;
                // checks if the weapon type needs to wrap around
                if (m_iWeaponType == 3)
                {
                    m_iWeaponType = 0;
                }
                else
                {
                    // increments the weapon type
                    m_iWeaponType++;
                }
                // resets the timers
                m_fHorizontalTimer = m_fHorizontalDelay;
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a down input is given
            else if (m_gamePadState.ThumbSticks.Left.Y < -m_fInputPrerequisite || m_gamePadState.DPad.Down == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object based on the current colour
                switch (m_iHoverpodColour)
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
                // moves the indicator down
                m_indicators[1].SetActive(false);
                m_indicators[2].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if an up input is given
            else if (m_gamePadState.ThumbSticks.Left.Y > m_fInputPrerequisite || m_gamePadState.DPad.Up == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the dino type button
                m_iCurrentSelectable = 0;
                // moves the indicator up
                m_indicators[1].SetActive(false);
                m_indicators[0].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
        }
        // checks if the left or right weapon type button is selected, the delay is finished and the player has not yet confirmed
        else if ((m_iCurrentSelectable == 4 || m_iCurrentSelectable == 5) && m_fHorizontalTimer < 0.0f && !m_bConfirmed)
        {
            // sets the selected object to the weapon type button
            m_iCurrentSelectable = 3;
            // sets the button colour back to normal
            m_selectables[4].image.color = Color.white;
            m_selectables[5].image.color = Color.white;
        }
        // checks if the colour blue is selected, the input timer is finished and the player has not yet confirmed their character
        else if (m_iCurrentSelectable == 6 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            // checks if a left input is given
            if (m_gamePadState.ThumbSticks.Left.X < -m_fInputPrerequisite || m_gamePadState.DPad.Left == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected colour to yellow
                m_iCurrentSelectable = 9;
                // moves the colour indicator to yellow
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 3;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a right input is given
            else if (m_gamePadState.ThumbSticks.Left.X > m_fInputPrerequisite || m_gamePadState.DPad.Right == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected colour to red
                m_iCurrentSelectable = 7;
                // moves the colour indicator to red
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 1;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a down input is given
            else if (m_gamePadState.ThumbSticks.Left.Y < -m_fInputPrerequisite || m_gamePadState.DPad.Down == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the dino type button
                m_iCurrentSelectable = 0;
                // moves the indicator up to the dino type button
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a up input is given
            else if (m_gamePadState.ThumbSticks.Left.Y > m_fInputPrerequisite || m_gamePadState.DPad.Up == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the weapon type button
                m_iCurrentSelectable = 3;
                // moves the indicator up
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
        }
        // checks if the colour red is selected, the input timer is finished and the player has not yet confirmed their character
        else if (m_iCurrentSelectable == 7 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            // checks if a left input is given
            if (m_gamePadState.ThumbSticks.Left.X < -m_fInputPrerequisite || m_gamePadState.DPad.Left == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected colour to blue
                m_iCurrentSelectable = 6;
                // moves the colour indicator to blue
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 0;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a right input is given
            else if (m_gamePadState.ThumbSticks.Left.X > m_fInputPrerequisite || m_gamePadState.DPad.Right == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected colour to green
                m_iCurrentSelectable = 8;
                // moves the colour indicator to green
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 2;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a down input is given
            else if (m_gamePadState.ThumbSticks.Left.Y < -m_fInputPrerequisite || m_gamePadState.DPad.Down == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the dino type button
                m_iCurrentSelectable = 0;
                // moves the indicator up to the dino type button
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a up input is given
            else if (m_gamePadState.ThumbSticks.Left.Y > m_fInputPrerequisite || m_gamePadState.DPad.Up == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the weapon type button
                m_iCurrentSelectable = 3;
                // moves the indicator up
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
        }
        // checks if the colour green is selected, the input timer is finished and the player has not yet confirmed their character
        else if (m_iCurrentSelectable == 8 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            // checks if a left input is given
            if (m_gamePadState.ThumbSticks.Left.X < -m_fInputPrerequisite || m_gamePadState.DPad.Left == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected colour to red
                m_iCurrentSelectable = 7;
                // moves the colour indicator to red
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 1;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a right input is given
            else if (m_gamePadState.ThumbSticks.Left.X > m_fInputPrerequisite || m_gamePadState.DPad.Right == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected colour to yellow
                m_iCurrentSelectable = 9;
                // moves the colour indicator to yellow
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 3;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a down input is given
            else if (m_gamePadState.ThumbSticks.Left.Y < -m_fInputPrerequisite || m_gamePadState.DPad.Down == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the dino type button
                m_iCurrentSelectable = 0;
                // moves the indicator up to the dino type button
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a up input is given
            else if (m_gamePadState.ThumbSticks.Left.Y > m_fInputPrerequisite || m_gamePadState.DPad.Up == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the weapon type button
                m_iCurrentSelectable = 3;
                // moves the indicator up
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
        }
        // checks if the colour yellow is selected, the input timer is finished and the player has not yet confirmed their character
        else if (m_iCurrentSelectable == 9 && m_fInputTimer < 0.0f && !m_bConfirmed)
        {
            // checks if a left input is given
            if (m_gamePadState.ThumbSticks.Left.X < -m_fInputPrerequisite || m_gamePadState.DPad.Left == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected colour to green
                m_iCurrentSelectable = 8;
                // moves the colour indicator to green
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 2;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a right input is given
            else if (m_gamePadState.ThumbSticks.Left.X > m_fInputPrerequisite || m_gamePadState.DPad.Right == ButtonState.Pressed || Input.GetAxis("Horizontal" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected colour to blue
                m_iCurrentSelectable = 6;
                // moves the colour indicator to blue
                m_colourIndicators[m_iHoverpodColour].SetActive(false);
                m_iHoverpodColour = 0;
                m_colourIndicators[m_iHoverpodColour].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a down input is given
            else if (m_gamePadState.ThumbSticks.Left.Y < -m_fInputPrerequisite || m_gamePadState.DPad.Down == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) < 0.0f)
            {
                // sets the selected object to the dino type button
                m_iCurrentSelectable = 0;
                // moves the indicator up to the dino type button
                m_indicators[2].SetActive(false);
                m_indicators[0].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
            // checks if a up input is given
            else if (m_gamePadState.ThumbSticks.Left.Y > m_fInputPrerequisite || m_gamePadState.DPad.Up == ButtonState.Pressed || Input.GetAxis("Vertical" + m_cPlayerNumber) > 0.0f)
            {
                // sets the selected object to the weapon type button
                m_iCurrentSelectable = 3;
                // moves the indicator up
                m_indicators[2].SetActive(false);
                m_indicators[1].SetActive(true);
                // resets the timer
                m_fInputTimer = m_fInputDelay;
            }
        }

        // checks if the player wants to confirm their player, the colour they want is available, the input timer is finished and the player has not yet confirmed their character
        if ((m_gamePadState.Buttons.A == ButtonState.Pressed || Input.GetAxis("Fire" + m_cPlayerNumber) != 0.0f) && m_colourPicker.IsAvailable(m_iHoverpodColour) && !m_bConfirmed && m_fInputTimer < 0.0f)
        {
            // sets the colour to unavailable
            m_colourPicker.SetOtherAvailability(false, m_iHoverpodColour);
            // confirms the player
            m_bConfirmed = true;
            // shows the confirm screen
            m_confirmCanvas.SetActive(true);
            // stores the player preferences in the character manager
            CharacterManager.Instance.m_hoverpodTypes[m_cPlayerNumber - 1] = m_hoverpodTypes[m_iHoverpodType];
            CharacterManager.Instance.m_hoverpodColours[m_cPlayerNumber - 1] = m_hoverpodColours.rows[m_iHoverpodColour].row[m_iHoverpodType];
            CharacterManager.Instance.m_nDinoColourIndex[m_cPlayerNumber - 1] = m_iHoverpodColour;
            CharacterManager.Instance.m_nDinoTypeIndex[m_cPlayerNumber - 1] = m_iHoverpodType;
            // increments the amount of ready players
            GetComponentInParent<PlayGame>().m_cReadyPlayers++;
            // resets the timer
            m_fInputTimer = m_fInputDelay;
        }
        // checks if the player wants to leave the confirm screen, the input timer is finished and the player has confirmed their character
        if ((m_gamePadState.Buttons.B == ButtonState.Pressed || Input.GetAxis("Pause" + m_cPlayerNumber.ToString()) != 0.0f) && m_bConfirmed && m_fInputTimer < 0.0f)
        {
            // sets the colour to available
            m_colourPicker.SetOtherAvailability(true, m_iHoverpodColour);
            // unconfirms the player
            m_bConfirmed = false;
            // hides the confirm screen
            m_confirmCanvas.SetActive(false);
            // decrements the amount of ready players
            GetComponentInParent<PlayGame>().m_cReadyPlayers--;
            // resets the timer
            m_fInputTimer = m_fInputDelay;
        }
        // checks if the player wants to leave the character selection screen, the input timer is finished
        else if ((m_gamePadState.Buttons.B == ButtonState.Pressed || Input.GetAxis("Pause" + m_cPlayerNumber.ToString()) != 0.0f) && m_fInputTimer < 0.0f)
        {
            // activates the game setup canvas
            transform.parent.GetComponentInParent<ActivateMenu>().GameSetup();
            // deactivates the character selection screen
            transform.parent.gameObject.SetActive(false);
            // resets the timer
            m_fInputTimer = m_fInputDelay;
        }

        // moves the colour to the next available one
        while (!m_colourPicker.IsAvailable(m_iHoverpodColour))
        {
            // moves the current selected index
            m_iCurrentSelectable++;
            if (m_iCurrentSelectable == 10)
            {
                m_iCurrentSelectable = 6;
            }
            m_colourIndicators[m_iHoverpodColour].SetActive(false);
            // moves the current colour
            m_iHoverpodColour++;
            if (m_iHoverpodColour == 4)
            {
                m_iHoverpodColour = 0;
            }
            m_colourIndicators[m_iHoverpodColour].SetActive(true);
        }

        // sets the text of the dino type button based on the current selected dino type
        m_selectables[0].GetComponentInChildren<Text>().text = m_sDinoTypes[m_iHoverpodType];
        // sets the text of the weapon type button based on the current selected weapon type
        m_selectables[3].GetComponentInChildren<Text>().text = m_sWeaponTypes[m_iWeaponType];

        // updates the display of the character based on the current selected dino type and colour
        m_playerViewer.GetComponent<MeshFilter>().mesh = m_hoverpodTypes[m_iHoverpodType];
        m_playerViewer.GetComponent<MeshRenderer>().material = m_hoverpodColours.rows[m_iHoverpodColour].row[m_iHoverpodType];
    }

    // resets the character selection screen
    public void Reset()
    {
        m_bConfirmed = false;
        m_confirmCanvas.SetActive(false);
        m_iCurrentSelectable = 0;
        m_indicators[0].SetActive(true);
        m_indicators[1].SetActive(false);
        m_indicators[2].SetActive(false);
        m_colourIndicators[m_iHoverpodColour].SetActive(false);

        m_iHoverpodType = m_cPlayerNumber - 1;
        m_iWeaponType = m_cPlayerNumber - 1;
        m_iHoverpodColour = m_cPlayerNumber - 1;
        m_colourIndicators[m_iHoverpodColour].SetActive(true);
        m_colourPicker.SetOtherAvailability(true, m_iHoverpodColour);

        m_fInputTimer = m_fInputDelay;
        m_fHorizontalTimer = 0.0f;

        GetComponentInParent<PlayGame>().m_cReadyPlayers = 0;
    }
}