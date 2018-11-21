using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : PersistantSingleton<CharacterManager>
{
    // collection of references to the hoverpod meshs
    [HideInInspector]
    public Mesh[] m_hoverpodTypes = new Mesh[4];
    // collection of references to the hoverpod materials
    [HideInInspector]
    public Material[] m_hoverpodColours = new Material[4];
    // collection of references to the dino meshs
    [HideInInspector]
    public int[] m_iDinoTypes = new int[4];
    // collection of references to the dino materials
    [HideInInspector]
    public int[] m_iDinoColours = new int[4];
    // collection of references to the dino materials
    [HideInInspector]
    public Material[] m_dinoColours = new Material[4];
    // stores index of dino type selected
    [HideInInspector]
    public int[] m_nDinoTypeIndex = new int[4];
    // stores colour of dino type selected
    [HideInInspector]
    public int[] m_nDinoColourIndex = new int[4];
    // determines which players are playing
    [HideInInspector]
    public bool[] m_bActivePlayers = new bool[4];
}