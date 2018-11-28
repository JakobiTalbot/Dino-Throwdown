using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundOnAwake : MonoBehaviour
{
	// Use this for initialization
	void Awake()
    {
        // get audio sources
        AudioSource[] m_audioSources = GetComponents<AudioSource>();

        // get random audio index
        int nRandomAudio = Random.Range(0, m_audioSources.Length - 1);

        // set audio volume
        if (OptionsManager.InstanceExists)
            m_audioSources[nRandomAudio].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;

        // play audio
        m_audioSources[nRandomAudio].Play();
        Debug.Log(nRandomAudio);
	}
}