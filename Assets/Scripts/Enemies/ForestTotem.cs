using System.Collections;
using UnityEngine;

public class ForestTotem : Entity
{
    //Settings
    public float offensiveDistance;
    public float spawnPointDistanceLimit;
    public float attackRange;
    public float attackCooldown;
    public float rotatingSpeed;
    public float movementSpeed;
    public int damage;

    //Animations
    private static readonly int sleepHash = Animator.StringToHash("Sleep");
    private static readonly int idleHash = Animator.StringToHash("Idle");
    private static readonly int awakeHash = Animator.StringToHash("Awake");
    private static readonly int moveHash = Animator.StringToHash("Move");
    private static readonly int attack1Hash = Animator.StringToHash("Attack 1");
    private static readonly int attack2Hash = Animator.StringToHash("Attack 2");
    private static readonly int deathHash = Animator.StringToHash("Death");
    private static readonly int parryHash = Animator.StringToHash("Parry");

    //Sounds
    [SerializeField] private AudioClip attack1;
    [SerializeField] private AudioClip attack2;
    [SerializeField] private AudioClip awaken;
    [SerializeField] private AudioClip noises;
    [SerializeField] private AudioSource noiseSource;
    [SerializeField] private AudioSource movementAudio;

    private bool awake = false;
    private bool offensive = false;
    private bool attacking = false;
    private bool inAttackCooldown = false;
    private bool stunned = false;

    private Transform player;
    private Vector3 spawnPoint;
    private float distance;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private CollisionRegister sword;

    private float currentOffensiveDistance, spawnPointDistance;

    private bool awakeAudioOneShot = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentOffensiveDistance = offensiveDistance;
        spawnPoint = transform.position;
    }

    void Update()
    {
        if(!alive) return;

        distance = Vector3.Distance(player.position, transform.position);
        spawnPointDistance = Vector3.Distance(transform.position, spawnPoint);
        
        if (awake)
        {
            CheckAround();

            if(attacking)
            {
                if(sword.playerCollided)
                {
                    sword.playerCollided = false;
                    sword.enabled = false;

                    if(CombatHandler.Instance.TakeHit(damage))
                        StartCoroutine(Stun());
                }

                return;
            }

            if(stunned) return;

            if(offensive)
                FollowPlayer();
            else 
                BackToSpawnPoint();
        }
        else
            TryToAwake();
    }

    private void CheckAround()
    {
        if (distance < currentOffensiveDistance && spawnPointDistance < spawnPointDistanceLimit)
            offensive = true;
        else 
            offensive = false;
    }

    private void TryToAwake()
    {
        if (distance <= offensiveDistance)
            StartCoroutine(WakeUp());
    }

    private void FollowPlayer()
    {
        bool inRange = FollowTarget(player.position, attackRange * 0.9f);

        if(distance < attackRange) Attack();
    }

    private void BackToSpawnPoint()
    {
        bool inRange = FollowTarget(spawnPoint, 2f);

        if(inRange)
            currentOffensiveDistance = offensiveDistance;
        else
            currentOffensiveDistance = 2;
    }

    private bool FollowTarget(Vector3 targetPos, float distanceToStop)
    {
        LookAtTarget(targetPos);

        float distance = Vector3.Distance(targetPos, transform.position);

        if(distance < distanceToStop)
        {
            anim.CrossFade(idleHash, 0, 0);
            movementAudio.Pause();
            return true;
        }
        else
        {
            anim.CrossFade(moveHash, 0, 0);
            if(!movementAudio.isPlaying) movementAudio.Play();
            movementAudio.UnPause();
            rigid.velocity = transform.forward * movementSpeed + transform.up * rigid.velocity.y;
            return false;
        }
    }

    private void LookAtTarget(Vector3 targetPos)
    {
        var lookPos = targetPos - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
    }

    private void Attack()
    {
        if(inAttackCooldown) return;

        movementAudio.Pause();
        LookAtTarget(player.position);
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        attacking = true;

        int rand = Random.Range(1, 3);

        if(rand == 1)
        {
            anim.CrossFade(attack1Hash, 0, 0);
            AudioManager.Instance.PlayOneShot3D(attack1, gameObject, AudioManager.AudioType.SFX, 1);
        }
        else if(rand == 2)
        {
            anim.CrossFade(attack2Hash, 0, 0);
            AudioManager.Instance.PlayOneShot3D(attack2, gameObject, AudioManager.AudioType.SFX, 1);
        }

        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length + 0.5f);
        StartCoroutine(AttackCoolDownCoroutine());
        attacking = false;
    }

    private IEnumerator WakeUp()
    {
        if(awakeAudioOneShot)
        {
            AudioManager.Instance.PlayOneShot3D(awaken, gameObject, AudioManager.AudioType.SFX, 1);
            AudioManager.Instance.PlayOnAudioSorce(noises, noiseSource, AudioManager.AudioType.SFX, 1);
            awakeAudioOneShot = false;
        }

        anim.CrossFade(awakeHash, 0, 0);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length * 4);
        rigid.isKinematic = false;
        rigid.useGravity = true;
        awake = true;
    }

    private IEnumerator AttackCoolDownCoroutine()
    {
        inAttackCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        sword.enabled = true;
        inAttackCooldown = false;
    }

    private IEnumerator Stun()
    {
        anim.CrossFade(parryHash, 0, 0);
        stunned = true;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);
        stunned = false;
    }

    public override void Die()
    {
        base.Die();
        anim.CrossFade(deathHash, 0, 0);
        AudioManager.Instance.PlayOneShot3D(awaken, gameObject, AudioManager.AudioType.SFX, 1);
    }
}
