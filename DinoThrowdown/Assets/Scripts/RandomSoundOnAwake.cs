using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundOnAwake : MonoBehaviour
{
    public AudioSource[] m_audioSources;
    // Use this for initialization
    void Start()
    {
        // get random audio index
        int nRandomAudio = Random.Range(0, m_audioSources.Length);

        // set audio volume
        if (OptionsManager.InstanceExists)
            m_audioSources[nRandomAudio].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;

        // play audio
        m_audioSources[nRandomAudio].enabled = true;
    }
}