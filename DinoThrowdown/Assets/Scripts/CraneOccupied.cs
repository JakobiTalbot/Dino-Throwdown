using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneOccupied : MonoBehaviour
{
    // determines if there is a player at the crane controller
    [HideInInspector]
    public bool m_bOccupied = false;
    // reference to a player
    [HideInInspector]
    public GameObject m_player;
}