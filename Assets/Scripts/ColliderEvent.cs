using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Collision> onCollisionEnter;
    [HideInInspector] public UnityEvent<Collision> onCollisionStay;
    [HideInInspector] public UnityEvent<Collision> onCollisionExit;
    [HideInInspector] public UnityEvent<Collider> onTriggerEnter;
    [HideInInspector] public UnityEvent<Collider> onTriggerStay;
    [HideInInspector] public UnityEvent<Collider> onTriggerExit;

    void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter.Invoke(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        onCollisionStay.Invoke(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        onCollisionExit.Invoke(collision);
    }

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke(other);
    }

    void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(other);
    }
}
