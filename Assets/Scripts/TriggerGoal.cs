using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if ("puck".Equals(other.tag))
        {
            ResetScript resetScript = other.GetComponent<ResetScript>();
            if (resetScript != null) resetScript.ResetObject();
        }
    }
}
