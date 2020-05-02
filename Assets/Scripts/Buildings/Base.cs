﻿using UnityEngine;

[RequireComponent(typeof(DamageableBehaviour))]
[RequireComponent(typeof(OnMouseOverInfoPanel))]
public class Base : MonoBehaviour, ITarget, IDamageable
{
    [SerializeField]
    private PlayerManager owner;
    [SerializeField]
    private BaseConfiguration config;
    private float incomePerSecond;
    /// <summary>
    /// Damage logic keeper.
    /// </summary>
    private DamageableBehaviour damageableBehaviour;
    /// <summary>
    /// GUI panel for showing information about the base.
    /// </summary>
    [SerializeField]
    private BaseInfoPanel panel;

    public Transform TargetPoint => transform;
    public PlayerManager Owner => owner;

    /// <summary>
    /// Recieves damage. If destroyed, the owner loses.
    /// </summary>
    public void ReceiveDamage(float damage, PlayerManager from)
    {
        if (damageableBehaviour.ReceiveDamage(damage))
        {
            Destroy(this.gameObject);
            LevelManager.Instance.EndGame(from, owner);
        }
    }
    /// <summary>
    /// For heal button.
    /// </summary>
    public void ReceiveHeal(float heal, int cost)
    {
        if (cost <= owner.Money)
        {
            damageableBehaviour.ReceiveHeal(heal);
            owner.SpendMoney(cost);
        }
    }

    private void Start()
    {
        damageableBehaviour = GetComponent<DamageableBehaviour>();
        damageableBehaviour.healthText = panel.healthLabel;
        incomePerSecond = config.incomePerSecond;
        owner.IncreaseIncome(incomePerSecond);

        // Initializing info panel with valid data.
        GetComponent<OnMouseOverInfoPanel>().panel = panel;
        string health = ((int)damageableBehaviour.Health).ToString();
        panel.maxHealth.text = health;
        panel.healthLabel.text = health;
        panel.regeneration.text = damageableBehaviour.Regeneration.ToString("0.##");
        panel.energyIncome.text = incomePerSecond.ToString("0.##");
    }
}