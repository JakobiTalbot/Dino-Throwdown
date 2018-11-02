using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour
{
    public float m_fFallSpeed = 10.0f;

    private void Update()
    {
        GetComponent<Rigidbody>().velocity += Vector3.down * m_fFallSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), collision.collider);
        }
    }
}