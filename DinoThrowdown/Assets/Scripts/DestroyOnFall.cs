using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FallOff")
            Destroy(gameObject);
    }
}