using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float baseDamage = 0.0f;
    [SerializeField] float timeDamageScale = 0.0f;
    [SerializeField] float maxTimeDamage = 0.0f;
    [SerializeField] float minVelocity = 0.0f;
    [SerializeField] float maxVelocity = 0.0f;
    [SerializeField] float minVelocityDamage = 0.0f;
    [SerializeField] float maxVelocityDamage = 0.0f;
    [SerializeField] bool destroyOnCollision = false;

    Rigidbody rb;

    float timeDamage = 0.0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        timeDamage += timeDamageScale * Time.fixedDeltaTime;
        timeDamage = Mathf.Clamp(timeDamage, 0.0f, maxTimeDamage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform);
        if (collision.collider.GetComponent<HealthTarget>())
        {
            //Debug.Log(collision.collider);
            Vector3 velocity = rb.velocity;
            if (collision.collider.GetComponentInParent<Rigidbody>())
            {
                velocity -= collision.collider.GetComponentInParent<Rigidbody>().velocity;
            }
            if (velocity.magnitude >= minVelocity)
            {
                float damage = baseDamage + timeDamage;
                float alpha = (velocity.magnitude - minVelocity) / (maxVelocity - minVelocity);
                float velocityDamage = Mathf.Lerp(minVelocityDamage, maxVelocityDamage, alpha);
                damage += velocityDamage;
                collision.collider.GetComponent<HealthTarget>().Damage(damage);
            }
        }

        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}
