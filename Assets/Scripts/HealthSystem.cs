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
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip damageSound;

    [HideInInspector] public UnityEvent<float> damageEvent;
    bool regenCooldown = false;
    bool hurtCooldown = false;

    Vector3 spawnPoint = Vector3.zero;

    private void Awake()
    {
        damageEvent.AddListener(Damage);
        health = maxHealth;
        spawnPoint = transform.position;
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

            source.PlayOneShot(damageSound);

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
        source.PlayOneShot(deathSound);
        transform.position = transform.position + Vector3.up * 50.0f;
        GetComponent<Rigidbody>().isKinematic = true;
        Invoke(nameof(Respawn), 5.0f);
    }

    void Respawn()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        health = maxHealth;
        transform.position = spawnPoint;
    }
}
