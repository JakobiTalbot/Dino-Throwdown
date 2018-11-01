using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsWreckingBowl : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        // checks if the collider is a player
        if (other.CompareTag("Player"))
        {
            if (OptionsManager.InstanceExists)
            {
                other.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
            }

            // plays the audio
            other.GetComponent<AudioSource>().Play();

            // sets the player as out
            other.GetComponent<PlayerController>().m_bIsOut = true;
        }
    }
}