using UnityEngine;

public class ResetScript : MonoBehaviour
{
    private Vector3 _originalPoint;

    private void Awake()
    {
        _originalPoint = transform.position;
    }

    public void ResetObject()
    {
        Debug.Log("Reset Object");
        transform.position = _originalPoint;
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.velocity = Vector3.zero;
    }
}