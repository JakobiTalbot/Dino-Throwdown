using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundOnAwake : MonoBehaviour
{
    public AudioSource[] m_audioSources;

    private float[] m_fOriginalVolumes;

    private void Awake()
    {
        m_fOriginalVolumes = new float[m_audioSources.Length];
        for (int i = 0; i < m_audioSources.Length; i++)
        {
            m_fOriginalVolumes[i] = m_audioSources[i].volume;
        }
    }

    private void Start()
    {
        // get random audio index
        int nRandomAudio = Random.Range(0, m_audioSources.Length);

        // set audio volume
        if (OptionsManager.InstanceExists)
            m_audioSources[nRandomAudio].volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume * m_fOriginalVolumes[nRandomAudio];

        // play audio
        m_audioSources[nRandomAudio].enabled = true;
    }
}