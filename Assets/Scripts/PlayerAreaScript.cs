using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAreaScript : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        ResetScript resetScript = other.GetComponent<ResetScript>();
        if (resetScript != null)
        {
            resetScript.ResetObject();
        }
    }
}
