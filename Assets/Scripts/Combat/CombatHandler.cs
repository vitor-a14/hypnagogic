using System.Collections;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public static CombatHandler Instance;

    [SerializeField] private Animator anim;
    private int currentAttack = 1;
    private Inputs controls;
    private bool attacking = false;
    private bool defending = false;
    private WeaponType currentWeapon;

    //Animation hash's
    private static readonly int defendHash = Animator.StringToHash("Defend");

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

    private void Attack()
    {
        //Check if the player is holding a weapon, is not defending nor attacking
        if(attacking || currentWeapon == null || defending) return;

        StopAllCoroutines(); //Reset all coroutines to avoid bugs

        //Get the animation based on the current combo
        string animationName = currentWeapon.weaponType + "_Attack" + currentAttack;
        int animHash = Animator.StringToHash(animationName);

        anim.CrossFade(animHash, 0, 0); //Play the animation

        StartCoroutine(AttackDuration(1 / currentWeapon.velocity)); //Attack cooldown
        StartCoroutine(RecoverDuration((1 / currentWeapon.velocity) + 0.3f)); //Animation duration cooldown

        //Combo nubmer handler
        currentAttack += 1;
        if(currentAttack > 2) currentAttack = 1;
    }

    private void Defend()
    {
        //Check if the player is holding and a weapon and it's not already defending
        if(currentWeapon == null || defending) return; 

        StopAllCoroutines(); //Reset all coroutines to avoid bugs

        anim.CrossFade(defendHash, 0, 0); //Play defending animation

        //Handle the variables
        defending = true;
        attacking = false;
        anim.SetBool("Recover", false);
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
