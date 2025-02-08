using UnityEngine;

public class Interactable : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab()
    {
        rb.isKinematic = true;
    }

    public void Drop()
    {
        rb.isKinematic = true;
    }

    public void Throw(Vector3 throwVector)
    {
        Drop();

        rb.linearVelocity = throwVector;
    }
}
