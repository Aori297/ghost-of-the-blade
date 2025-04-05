using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthStamina : MonoBehaviour
{
    public static PlayerHealthStamina Instance;

    // Health
    public float MaxHealth = 100f;
    public float currentHealth;
    public TextMeshProUGUI health;
    public event Action OnHealthUpdate;


    // Stamina
    [SerializeField] private float MaxStamina = 100f;
    [SerializeField] private float StaminaRegenRate = 6f;
    public TextMeshProUGUI stamina;
    public float currentStamina;
    public float attackStamina = 2f;
    public float dashStamina = 5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        OnHealthUpdate += UpdateUI;
        currentStamina = 0;
        InvokeRepeating("RegenStamina", 0f, 2.0f);
    }

    void RegenStamina()
    {
        if (currentStamina >= MaxStamina) return;
        
        this.currentStamina += StaminaRegenRate;
        UpdateUI();
    }

    public void DepleteStamina(float DeltaStamina)
    {
        if (currentStamina <= 0) return;

        this.currentStamina -= DeltaStamina;
        UpdateUI();
    }
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount,MaxHealth);
        OnHealthUpdate?.Invoke();
    }

    void UpdateUI()
    {
        health.text = currentHealth.ToString();
        stamina.text = currentStamina.ToString();
    }
}
