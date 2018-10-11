﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    // speed at which the claw drops
    public float m_fMoveSpeed = 10.0f;
    // reference to the crane
    public CraneOccupied m_crane;
    // determines if the claw has dropped
    public bool m_bDropped = false;

    // drops the claw
    private void Drop()
    {
        // checks if the claw is above the target height
        if (transform.position.y > 5.5f)
        {
            // moves the claw down
            transform.Translate(-transform.up * Time.deltaTime * m_fMoveSpeed);
        }
        else
        {
            // sets the dropped status to true
            m_bDropped = true;
        }
    }
    // raises the claw
    private bool Raise()
    {
        // check is the claw is below the target height
        if (transform.position.y < 15.0f)
        {
            // moves the claw up
            transform.Translate(transform.up * Time.deltaTime * m_fMoveSpeed);
            return true;
        }
        else
        {
            m_bDropped = false;
            return false;
        }
    }
    // drops then rises
    public bool Grab()
    {
        // checks if the claw has already dropped
        if (!m_bDropped)
        {
            Drop();
        }
        else
        {
            return Raise();
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // checks if the claw collides with a player
        if (other.CompareTag("Player"))
        {
            // gets the player controller from the player
            PlayerController playerController = other.GetComponent<PlayerController>();
            // sets the player to picked up
            playerController.m_bPickedUp = true;
            playerController.m_bIsOut = true;
            // gives the player control over the claw
            playerController.m_claw = gameObject;
        }
    }
}