using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    [Header("Life")]
    public int maxLife;
    public float currentLife;

    [Header("Stamina")]
    public int maxStamina;
    public float currentStamina;
    public int staminaRecovery;
    [HideInInspector] public float staminaRecoveryTimer;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        currentLife = maxLife;
        currentStamina = maxStamina;
    }

    void LateUpdate()
    {
        StaminaManagement();
    }

    private void StaminaManagement()
    {
        if(currentStamina > 0 && PlayerController.Instance.isRunning)
            currentStamina -= Time.deltaTime;
        else if(currentStamina <= 0)
            PlayerController.Instance.isRunning = false;

        if(staminaRecoveryTimer >= staminaRecovery)
        {
            if(currentStamina < maxStamina)
                currentStamina += 2 * Time.deltaTime;
        }
        else if(!PlayerController.Instance.isRunning)
            staminaRecoveryTimer += Time.deltaTime;
    }

    public void DecreaseCurrentLife(int amount)
    {
        currentLife -= amount;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
    }

    public void IncreaseCurrentLife(int amount)
    {
        currentLife += amount;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
    }

    public void IncreaseMaxLife(int amount)
    {
        maxLife += amount;
        currentLife += amount;
        HUDManager.Instance.healthBarSlider.maxValue = maxLife;
    }

    public void DecreaseMaxLife(int amount)
    {
        maxLife -= amount;
        currentLife -= amount;
        currentLife = Mathf.Clamp(currentLife, 1, maxLife);
        HUDManager.Instance.healthBarSlider.maxValue = maxLife;
    }
}
