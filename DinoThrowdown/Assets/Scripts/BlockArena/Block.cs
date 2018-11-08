using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // speed at which the block accelerates down
    public float m_fFallAccelSpeed = 10.0f;

    private GameObject m_dropper;

    private void Awake()
    {
        m_dropper = GameObject.FindGameObjectWithTag("BombDropper");
    }

    private void Update()
    {
        // accelerate down
        GetComponent<Rigidbody>().velocity += Vector3.down * m_fFallAccelSpeed * Time.deltaTime;

        // remove transform from blocks array if not kinematic
        if (!GetComponent<Rigidbody>().isKinematic 
            && m_dropper.GetComponent<BombDropper>().m_blocks.Contains(transform))
        {
            m_dropper.GetComponent<BombDropper>().m_blocks.Remove(transform);
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