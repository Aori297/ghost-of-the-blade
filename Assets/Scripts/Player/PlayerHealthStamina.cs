using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthStamina : MonoBehaviour
{
    public static PlayerHealthStamina Instance;

    public float MaxHealth = 100f;
    public float currentHealth;
    public event Action OnHealthUpdate;

    public Slider healthSlider;
    public Slider staminaSlider;

    [SerializeField] private float MaxStamina = 100f;
    [SerializeField] private float StaminaRegenRate = 6f;
    [SerializeField] private float HealthRegenRate = 3f;
    public float currentStamina;
    public float attackStamina = 2f;
    public float dashStamina = 5f;

    public bool isBlocking;
    [SerializeField] SpriteRenderer playerSprite;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        OnHealthUpdate += UpdateUI;
        InvokeRepeating("RegenStamina", 0f, 2.0f);
        InvokeRepeating("RegenHealth", 0f, 3.0f);
    }

    void RegenHealth()
    {
        if (currentHealth >= MaxHealth) return;

        this.currentHealth += HealthRegenRate;
        UpdateUI();
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
        healthSlider.value = currentHealth;
        staminaSlider.value = currentStamina;
    }

    public int enemyDamageAmount = 10;
    public void TakeDamage(int amount, float waitSeconds)
    {
        if (!isBlocking)
        {
            enemyDamageAmount = amount;
            StartCoroutine(DamageDetection(waitSeconds));
          
        }
    }

    private IEnumerator DamageDetection(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        currentHealth = Mathf.Min(currentHealth - enemyDamageAmount, MaxHealth);
        OnHealthUpdate?.Invoke();

        float duration = 1f;
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.PingPong(time * (1f / duration) * 10f, 1f);
            playerSprite.color = Color.Lerp(Color.red, Color.white, t);

            time += Time.deltaTime;
            yield return null;
        }

        playerSprite.color = Color.white;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(enemyDamageAmount, 0);
            Destroy(collision.gameObject);

        }
    }

}
