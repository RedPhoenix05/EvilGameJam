using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] float invincibilityTime = 0.25f;
    [SerializeField] float healthRegenDelay = 5.0f;
    [SerializeField] float healthRegenRate = 10.0f;
    public float health = 0;

    [HideInInspector] public UnityEvent<float> damageEvent;
    bool regenCooldown = false;
    bool hurtCooldown = false;

    private void Awake()
    {
        damageEvent.AddListener(Damage);
        health = maxHealth;
    }

    private void FixedUpdate()
    {
        if (!regenCooldown)
        {
            health += healthRegenRate * Time.fixedDeltaTime;
            health = Mathf.Clamp(health, 0.0f, maxHealth);
        }
    }

    public void Damage(float damage)
    {
        if (!hurtCooldown && damage > 0.0f)
        {
            hurtCooldown = true;
            Invoke(nameof(DisableHurtCooldown), invincibilityTime);


            health -= damage;
            if (health <= 0.0f)
            {
                Die();
            }

            regenCooldown = true;
            CancelInvoke(nameof(DisableRegenCooldown));
            Invoke(nameof(DisableRegenCooldown), healthRegenDelay);
        }
    }

    private void DisableRegenCooldown()
    {
        regenCooldown = false;
    }

    private void DisableHurtCooldown()
    {
        hurtCooldown = false;
    }

    public void Die()
    {
        Debug.Log(gameObject + " has died.");
    }
}
