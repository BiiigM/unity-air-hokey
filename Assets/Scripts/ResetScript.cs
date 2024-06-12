using UnityEngine;

public class ResetScript : MonoBehaviour
{
    private Vector3 _originalPoint;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _originalPoint = transform.position;
        _originalRotation = transform.rotation;
    }

    public void ResetObject()
    {
        Debug.Log("Reset Object");
        transform.position = _originalPoint;
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.velocity = Vector3.zero;
        transform.rotation = _originalRotation;
    }
}