using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTarget : MonoBehaviour
{
    [SerializeField] float damageModifier = 1.0f;
    [SerializeField] ColliderEvent collisionEventHandler;

    HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponentInParent<HealthSystem>() ? GetComponentInParent<HealthSystem>() : null;

        if (!collisionEventHandler)
        {
            collisionEventHandler = GetComponent<ColliderEvent>() ? GetComponent<ColliderEvent>() : null;
        }
    }

    public void Damage(float damage)
    {
        if (healthSystem)
        {
            healthSystem.damageEvent.Invoke(damage * damageModifier);
        }
    }
}
