using System.Collections;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public static CombatHandler Instance;
    public float damageReductionInDefense;

    [SerializeField] private Animator anim;
    private int currentAttack = 1;
    private Inputs controls;
    [HideInInspector] public bool attacking = false;
    [HideInInspector] public bool defending = false;
    private WeaponType currentWeapon;
    private bool waiting;

    [Header("Audio")]
    [SerializeField] private AudioClip[] swordWoosh;
    [SerializeField] private AudioClip defend;
    [SerializeField] private AudioClip parry;
    [SerializeField] private AudioClip[] swordHitDefend;
    [SerializeField] private AudioClip[] takenHit;

    private float timeDefending;
    private bool targetWasHit = false;

    //Animation hash's
    private static readonly int defendHash = Animator.StringToHash("Defend");
    private static readonly int defenseReactHash = Animator.StringToHash("Defense React");

    private void Awake() 
    {
        //Set a global static instance for weapons to acess
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");

        //Set another input thread to handle the combat inputs
        controls = new Inputs();
        controls.Enable();

        controls.Player.Attack.performed += ctx => Attack();
        controls.Player.Defend.performed += ctx => Defend();
        controls.Player.Defend.canceled += ctx => StopDefending();
    }

    private void LateUpdate() 
    {
        if(defending)
            timeDefending += Time.deltaTime;
        else if(attacking && !targetWasHit && currentWeapon.target != null) // hit
        {
            AudioManager.Instance.PlayOneShot3D(currentWeapon.hitAudio, currentWeapon.gameObject, AudioManager.AudioType.SFX, 1);
            currentWeapon.target.TakeHit(currentWeapon.damage);
            targetWasHit = true;
        }
    }

    private void Attack()
    {
        //Check if the player is holding a weapon, is not defending nor attacking
        if(attacking || currentWeapon == null || defending) return;

        if(DialogueManager.Instance.dialogueIsPlaying || InventoryManager.Instance.onInventory) return;

        StopAllCoroutines(); //Reset all coroutines to avoid bugs

        PlayerController.Instance.isRunning = false;

        //Get the animation based on the current combo
        string animationName = currentWeapon.weaponType + "_Attack" + currentAttack;
        int animHash = Animator.StringToHash(animationName);

        anim.CrossFade(animHash, 0, 0); //Play the animation

        StartCoroutine(AttackDuration(1 / currentWeapon.velocity)); //Attack cooldown
        StartCoroutine(RecoverDuration((1 / currentWeapon.velocity) + 0.3f)); //Animation duration cooldown

        AudioClip swordWooshAudio = swordWoosh[Random.Range(0, swordWoosh.Length - 1)];
        AudioManager.Instance.PlayOneShot3D(swordWooshAudio, currentWeapon.gameObject, AudioManager.AudioType.SFX, 1);

        //Combo nubmer handler
        currentAttack += 1;
        if(currentAttack > 2) currentAttack = 1;
    }

    private void Defend()
    {
        //Check if the player is holding and a weapon and it's not already defending
        if(currentWeapon == null || defending) return; 

        if(DialogueManager.Instance.dialogueIsPlaying || InventoryManager.Instance.onInventory) return;

        StopAllCoroutines(); //Reset all coroutines to avoid bugs
        PlayerController.Instance.isRunning = false;

        if(anim.GetCurrentAnimatorClipInfo(0).GetHashCode() != defenseReactHash)
            anim.CrossFade(defendHash, 0, 0); //Play defending animation

        AudioManager.Instance.PlayOneShot3D(defend, currentWeapon.gameObject, AudioManager.AudioType.SFX, 1);

        //Handle the variables
        defending = true;
        attacking = false;
        timeDefending = 0;
        anim.SetBool("Recover", false);
    }

    //When the enemy lands a hit, call this function to check if the player will be damaged
    public bool TakeHit(int damage)
    {
        if(defending)
        {
            anim.CrossFade(defenseReactHash, 0, 0);

            //Normal defense
            if(timeDefending > 0.1f)
            {
                AudioManager.Instance.PlayOneShot3D(swordHitDefend[Random.Range(0, swordHitDefend.Length - 1)], currentWeapon.gameObject, AudioManager.AudioType.SFX, 1);
                //PlayerStatus.Instance.DecreaseCurrentLife((int)(damage * damageReductionInDefense));
                return false;
            }
            else //Perfect Defense (PARRY)
            {
                Effects.Instance.FreezeFrame();
                Effects.Instance.ScreenShake();
                AudioManager.Instance.PlayOneShot3D(parry, currentWeapon.gameObject, AudioManager.AudioType.SFX, 1);
                return true;
            }
        }
        else //Failed Defense (Player was attacked)
        {
            PlayerStatus.Instance.DecreaseCurrentLife(damage);
            AudioManager.Instance.PlayOneShot2D(takenHit[Random.Range(0, takenHit.Length - 1)], gameObject, AudioManager.AudioType.SFX, 1);

            if(InventoryManager.Instance.onInventory) InventoryManager.Instance.ListItems();
            if(DialogueManager.Instance.dialogueIsPlaying) DialogueManager.Instance.EndDialogue();

            return false;
        }
    }

    private void StopDefending()
    {
        if(!defending) return; //Only stop defending if the player is defending

        defending = false;
        anim.SetBool("Recover", true);
    }

    //Every weapon instance need to call this function!
    public void SetWeapon(WeaponType weapon)
    {
        currentWeapon = weapon;
        anim.speed = currentWeapon.velocity;
    }

    //Cooldown handling with coroutines
    private IEnumerator AttackDuration(float duration)
    {
        attacking = true;
        yield return new WaitForSeconds(duration);
        targetWasHit = false;
        attacking = false;
    }

    private IEnumerator RecoverDuration(float duration)
    {
        anim.SetBool("Recover", false);
        yield return new WaitForSeconds(duration);
        currentAttack = 1;
        anim.SetBool("Recover", true);
    }
}
