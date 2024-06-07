using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    private Vector3 _originalPoint;
    
    private void Awake()
    {
        _originalPoint = this.transform.position;
    }

    public void ResetObject()
    {
        this.transform.position = _originalPoint;
        Rigidbody rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
}
