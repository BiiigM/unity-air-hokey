using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 5f;
    public float sprintSpeed = 10f;

    Vector3 moveAxis;

    void Update()
    {
        moveAxis = new Vector3(Input.GetAxis("HorizontalP1"), 0, Input.GetAxis("VerticalP1"));
        moveAxis = Vector3.ClampMagnitude(moveAxis, 1);
    }

    private void FixedUpdate()
    {
        float walkSpeed = speed;
        Debug.Log(Input.GetAxis("SprintP1"));
        if (Input.GetAxis("SprintP1") == 1) {
            walkSpeed = sprintSpeed;
        }

        rb.MovePosition(rb.position + (moveAxis * speed * Time.fixedDeltaTime));
    }
}
