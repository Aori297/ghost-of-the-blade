using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthStamina : MonoBehaviour
{
    [SerializeField] private float MaxStamina = 10f;
    [SerializeField] private float StaminaRegenRate = 2f;

    public TMP_Text stamina;

    public float currentStamina;
    public float attackStamina = 2f;
    public float dashStamina = 5f;

    void Start()
    {
        currentStamina = 0;
        InvokeRepeating("RegenStamina", 0f, 2.0f);
    }

    void RegenStamina()
    {
        if (currentStamina >= MaxStamina) return;
        
        this.currentStamina += StaminaRegenRate;
        stamina.text = currentStamina.ToString();

        Debug.Log("Stamina: " + currentStamina);
    }

    public void DepleteStamina(float DeltaStamina)
    {
        if (currentStamina <= 0) return;
      
        this.currentStamina -= DeltaStamina;
        Debug.Log("Stamina: " + currentStamina);
    }
}
