using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    // reference to the crane
    public CraneManager[] m_crane;
    // reference to the crane seats
    public Transform[] m_seats;
    // duration of cheering
    public float m_fCheerDuration = 2.0f;

    // reference to dinosaurs in the audience
    private GameObject m_audience;
    // reference to the claw
    private Claw[] m_claw;
    // used to time how long the audience cheers for
    private Claw.PickupTimer m_cheerTimer;
    // stores all the animators of the audience
    private Animator[] m_anims;

    private void Start()
    {
        m_claw = new Claw[2];

        m_audience = GameObject.FindGameObjectWithTag("Audience");
        m_anims = m_audience.GetComponentsInChildren<Animator>();

        // gets and sets the claw component from each crane
        for (int i = 0; i < m_crane.Length; i++)
        {
            m_claw[i] = m_crane[i].GetComponentInChildren<Claw>();
        }

        m_cheerTimer.bFlag = false;
        m_cheerTimer.fTimer = m_fCheerDuration;
    }

    private void Update()
    {
        if (m_cheerTimer.bFlag)
        {
            m_cheerTimer.fTimer -= Time.deltaTime;
            if (m_cheerTimer.fTimer < 0.0f)
            {
                m_cheerTimer.bFlag = false;
                m_cheerTimer.fTimer = m_fCheerDuration;
                // stop cheer animation
                foreach (var dino in m_anims)
                {
                    dino.SetTrigger("Idle");
                }
            }
        }
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

            // play cheer animation
            foreach (var dino in m_anims)
            {
                dino.SetTrigger("Win");
            }
            m_cheerTimer.bFlag = true;

            PlayerController playerController = other.GetComponent<PlayerController>();

            if (OptionsManager.InstanceExists)
            {
                other.GetComponent<AudioSource>().volume = OptionsManager.Instance.m_fSFXVolume * OptionsManager.Instance.m_fMasterVolume;
            }

            // plays the audio
            other.GetComponent<AudioSource>().Play();

            // iterates through each crane
            for (int i = 0; i < m_crane.Length; i++)
            {
                // checks if the crane is occupied
                if (!m_crane[i].m_bOccupied)
                {
                    // sets the crane to occupied
                    m_crane[i].m_bOccupied = true;
                    m_crane[i].m_player = playerController.gameObject;

                    // sends the player to the crane
                    playerController.transform.position = m_seats[i].transform.position;

                    playerController.transform.localRotation = Quaternion.Euler(0.0f, m_seats[i].transform.rotation.eulerAngles.y, 0.0f);

                    // changes the status of the player to in the crane
                    playerController.m_bInCrane = true;
                    // gives the player control of the claw
                    playerController.m_claw = m_claw[i];

                    other.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
                    other.GetComponent<Rigidbody>().isKinematic = true;

                    return;
                }
            }

            // sets the player as out if all cranes are occupied
            playerController.m_bIsOut = true;
        }
    }
}