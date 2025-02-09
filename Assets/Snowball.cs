using Alteruna;
using AlterunaFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Snowball : MonoBehaviour
{
    public float baseDamage = 10.0f;
    public float timeScale = 10.0f;
    public ushort ownerID = 0; 

    float damage = 0.0f;
    Collider thisCollider;
    
    Spawner instantiator;

    private void Awake()
    {
        damage = baseDamage;
        thisCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        damage += timeScale * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Health target))
        {
            EfxManager.Instance.PlayImpact(collision.GetContact(0).point, collision.GetContact(0).normal, collision.transform, target.MaterialType);
            target.TakeDamage(ownerID, damage);
        }
        else
        {
            if (collision.collider.gameObject.layer == 0) // default layer
            {
                EfxManager.Instance.PlayImpact(collision.GetContact(0).point, collision.GetContact(0).normal, collision.transform);
            }
        }

        instantiator.Despawn(gameObject);
    }

    public void SetUp(Spawner instantiator, ushort ownerID, List<Collider> ignoredColliders, float baseDamage, float timeScale, Vector3 velocity)
    {
        this.instantiator = instantiator;

        this.ownerID = ownerID;
        this.baseDamage = baseDamage;
        this.timeScale = timeScale;
        GetComponent<Rigidbody>().velocity = velocity;

        foreach (Collider ignoredCollider in ignoredColliders)
        {
            Physics.IgnoreCollision(thisCollider, ignoredCollider);
        }
    }
}
