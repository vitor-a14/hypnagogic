using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDataPersistance
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
    [SerializeField] private GameObject damagedUIEffect;

    [Header("Consumables")]
    public int soulPotions;
    public int goldenSeeds;
    [SerializeField] private int healthRecoverAmount;
    [SerializeField] private float consumableCooldown;
    [SerializeField] private int maxSoulPotionsAllowed;
    [SerializeField] private int maxGoldenSeedsAllowed;

    [Header("UI and Audio")]
    [SerializeField] private AudioClip consumeSoulPotionAudio;
    [SerializeField] private TMP_Text soulPotionsText, goldenSeedsText;
    private bool consumableInCooldown = false;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        currentStamina = maxStamina;
    }

    private void Start() 
    {
        goldenSeedsText.text = goldenSeeds.ToString();
        soulPotionsText.text = soulPotions.ToString();
    }

    void LateUpdate()
    {
        StaminaManagement();
    }

    public void Save(ref Data gameData)
    {
        gameData.playerPosition = new float[3];
        gameData.playerPosition[0] = transform.position.x;
        gameData.playerPosition[1] = transform.position.y;
        gameData.playerPosition[2] = transform.position.z;

        gameData.maxLife = maxLife;
        gameData.currentLife = currentLife;
        gameData.soulPotions = soulPotions;
        gameData.goldenSeeds = goldenSeeds;
    }

    public void Load(Data gameData)
    {
        Vector3 playerPos = new Vector3(gameData.playerPosition[0], gameData.playerPosition[1], gameData.playerPosition[2]);
        PlayerController.Instance.rigid.position = playerPos;

        maxLife = gameData.maxLife;
        currentLife = gameData.currentLife;

        soulPotions = 0;
        goldenSeeds = 0;
        IncreaseSoulPotion(gameData.soulPotions);
        //INCREASE GOLDEN SEEDS
    }

    public void ConsumeSoulPotion()
    {
        if(soulPotions == 0 || consumableInCooldown || currentLife == maxLife) return;

        DecreaseSoulPotion(1);
        AudioManager.Instance.PlayOneShot2D(consumeSoulPotionAudio, gameObject, AudioManager.AudioType.SFX, 1);
        IncreaseCurrentLife(healthRecoverAmount);

        StartCoroutine(StartConsumableCooldown());
    }

    private IEnumerator StartConsumableCooldown()
    {
        consumableInCooldown = true;
        yield return new WaitForSeconds(consumableCooldown);
        consumableInCooldown = false;
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
        HUDManager.Instance.FadeInAndOut(damagedUIEffect, 0.3f, 15f);
        if(currentLife <=0)
            Die();
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

    public void IncreaseSoulPotion(int amout)
    {
        soulPotions += amout;
        soulPotions = Mathf.Clamp(soulPotions, 0, maxSoulPotionsAllowed);
        soulPotionsText.text = soulPotions.ToString();
    }

    public void DecreaseSoulPotion(int amout)
    {
        soulPotions -= 1;
        soulPotions = Mathf.Clamp(soulPotions, 0, maxSoulPotionsAllowed);
        soulPotionsText.text = soulPotions.ToString();
    }

    public void Die()
    {
        HUDManager.Instance.DeathScreen();
        CameraAnimation.Instance.ChangeAnimation(CameraAnimation.death);
        PlayerController.Instance.controls.Disable();
        CombatHandler.Instance.controls.Disable();
        StartCoroutine(FadeAudio());
        Effects.Instance.FreezeFrame(0.2f);
    }

    IEnumerator FadeAudio()
    {
        while(AudioListener.volume > 0)
        {
            AudioListener.volume -= Time.deltaTime;
            yield return null;
        }
    }
}
