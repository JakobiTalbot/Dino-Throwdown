using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwing : MonoBehaviour
{
    // calls the stop attack function
    // this function will be called by an event in animation
    public void StopAttack()
    {
        GetComponentInParent<PlayerController>().StopAttack();
    }
}