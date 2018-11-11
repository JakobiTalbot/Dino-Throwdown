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
    public Mesh[] m_dinoTypes = new Mesh[4];
    // collection of references to the dino materials
    [HideInInspector]
    public Material[] m_dinoColours = new Material[4];
}