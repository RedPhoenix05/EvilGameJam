using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [SerializeField] Collider grabCollider;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform attachTransform)
    {
        rb.isKinematic = true;

        transform.parent = attachTransform;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        grabCollider.enabled = false;
    }

    public void Drop()
    {
        rb.isKinematic = false;
        transform.parent = null;

        grabCollider.enabled = true;
    }

    public void Throw(Vector3 throwVector)
    {
        Drop();

        rb.velocity = throwVector;
    }
}
