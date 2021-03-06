﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject m_bombDropper;
    // speed at which the block accelerates down
    public float m_fFallAccelSpeed = 10.0f;

    private GameObject m_dropper;

    private void Awake()
    {
        GetComponent<Collider>().material.bounceCombine = PhysicMaterialCombine.Minimum;
    }
    private void Update()
    {
        // accelerate down
        GetComponent<Rigidbody>().velocity += Vector3.down * m_fFallAccelSpeed * Time.deltaTime;

        // remove transform from blocks array if not kinematic
        if (!GetComponent<Rigidbody>().isKinematic 
            && m_bombDropper.GetComponent<BombDropper>().m_blocks.Contains(transform))
        {
            m_bombDropper.GetComponent<BombDropper>().m_blocks.Remove(transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.collider);
        }
    }
}