using System.Collections;
using UnityEngine;

public class ForestTotem : Entity
{
    //Exclusive animations
    private static readonly int sleepHash = Animator.StringToHash("Sleep");
    private static readonly int awakeHash = Animator.StringToHash("Awake");
    private static readonly int attack1Hash = Animator.StringToHash("Attack 1");
    private static readonly int attack2Hash = Animator.StringToHash("Attack 2");

    [Header("Forest Totem Attributes")]
    [SerializeField] protected AudioClip passiveSound;
    [SerializeField] protected AudioSource passiveSoundSource;
    private bool awakeOneShot = true;

    void Start()
    {
        //Setup basic animations
        idleHash = Animator.StringToHash("Idle");
        moveHash = Animator.StringToHash("Move");
        deathHash = Animator.StringToHash("Death");
        parryHash = Animator.StringToHash("Parry"); 
    }

    void LateUpdate()
    {
        if(!active && playerInReach && life > 0) StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        if(awakeOneShot)
        {
            AudioManager.Instance.PlayOneShot3D(deathSound, gameObject, AudioManager.AudioType.SFX, 1);
            AudioManager.Instance.PlayOnAudioSorce(passiveSound, passiveSoundSource, AudioManager.AudioType.SFX, 1);
            anim.CrossFade(awakeHash, 0, 0);
            awakeOneShot = false;
        }

        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length * 4);
        rigid.isKinematic = false;
        rigid.useGravity = true;
        active = true;
    }
}
