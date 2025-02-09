using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [SerializeField] Collider grabCollider;
    [SerializeField] List<Collider> disableOnGrab = new();

    [SerializeField] bool teleportHome;
    [SerializeField] float teleportTimer = 20.0f;

    Rigidbody rb;
    Vector3 originalPosition = Vector3.zero;
    Quaternion originalRotation = Quaternion.identity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void Grab(Transform attachTransform)
    {
        rb.isKinematic = true;

        transform.parent = attachTransform;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        grabCollider.enabled = false;

        foreach (Collider collider in disableOnGrab)
        {
            collider.enabled = false;
        }

        CancelInvoke(nameof(TeleportHome));
    }

    public void Drop()
    {
        rb.isKinematic = false;
        transform.parent = null;

        grabCollider.enabled = true;

        foreach (Collider collider in disableOnGrab)
        {
            collider.enabled = true;
        }

        Invoke(nameof(TeleportHome), teleportTimer);
    }

    public void Throw(Vector3 throwVector, float throwAngularVelocity)
    {
        Drop();

        rb.velocity = throwVector;
        rb.angularVelocity = new Vector3(throwAngularVelocity * 360 * Mathf.Deg2Rad, 0.0f, 0.0f);
    }

    void TeleportHome()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        rb.isKinematic = true;
    }
}
