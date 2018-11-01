using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsWreckingBowl : MonoBehaviour
{
    private void Start()
    {
    }

    private void OnTriggerStay(Collider other)
    {
        // checks if the collider is a player
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Rigidbody>().isKinematic)
            {
                return;
            }

            PlayerController playerController = other.GetComponent<PlayerController>();

            if (OptionsManager.InstanceExists)
            {
                other.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
            }

            // plays the audio
            other.GetComponent<AudioSource>().Play();

            // sets the player as out if all cranes are occupied
            playerController.m_bIsOut = true;
        }
    }
}