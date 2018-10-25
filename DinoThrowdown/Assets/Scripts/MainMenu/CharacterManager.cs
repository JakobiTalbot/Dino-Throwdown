using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : PersistantSingleton<CharacterManager>
{
    // reference to the different meshes
    public Mesh[] m_dinoTypes;
    // collection of references to the different materials
    public Material[] m_colours;
}