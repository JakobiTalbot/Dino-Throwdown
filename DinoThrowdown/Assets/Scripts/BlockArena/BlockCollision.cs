using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    // rate at which the block falls
    public float m_fFallSpeed = 10.0f;

    private void Update()
    {
        // moves the block down
        GetComponent<Rigidbody>().velocity += Vector3.down * m_fFallSpeed * Time.deltaTime;
    }

    // ignores collision with other blocks
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.collider);
        }
    }
}