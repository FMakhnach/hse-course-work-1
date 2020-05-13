﻿using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Number of seconds after which the projectile self-destroys.
    /// </summary>
    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private float speed;
    private float damage;
    /// <summary>
    /// To check if we hit an enemy.
    /// </summary>
    private PlayerManager owner;
    /// <summary>
    /// Destroying this on collision.
    /// </summary>
    private ParticleSystem particles;
    /// <summary>
    /// direction * speed * Time.deltaTime
    /// </summary>
    private Vector3 directionTimesSpeed;


    /// <summary>
    /// Initializing a projectile by giving it all what it needs.
    /// </summary>
    public void Initialize(Vector3 direction, float damage, PlayerManager owner, ParticleSystem particles)
    {
        directionTimesSpeed = direction.normalized * speed * Time.deltaTime;
        this.owner = owner;
        this.damage = damage;
        this.particles = particles;
        // It should be destroyed in time.
        PoolManager.Instance.Reclaim(gameObject, lifeTime);
        PoolManager.Instance.Reclaim(particles.gameObject, lifeTime);
    }

    private void Update()
    {
        // Just flying in given direction.
        transform.Translate(directionTimesSpeed, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponentInParent<IDamageable>();
        // Check if we hit an enemy
        if (damageable != null && damageable.Owner != owner)
        {
            damageable.ReceiveDamage(damage, owner);
            PoolManager.Instance.Reclaim(particles.gameObject);
        }
    }
}