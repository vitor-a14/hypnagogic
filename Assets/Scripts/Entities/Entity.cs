using System.Collections;
using UnityEngine;

[System.Serializable]
public class AttackPattern
{
    public string attackAnimationName;
    public AudioClip attackAudio;
}

public class Entity : MonoBehaviour
{
    [Header("Attributes")]
    public bool active = false;
    public bool attackPlayer;
    public int life;
    public int damage;
    public float attackRange;
    public float movementSpeed;
    public float attackCooldown;
    public AttackPattern[] attacks;

    [Header("Components")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected CollisionRegister hitCollider;

    //States
    protected bool goingBackToSpawn = false;
    protected bool offensive = false; 
    protected bool inAttackCooldown = false;
    protected bool stunned = false; 
    protected bool playerInReach = false; 
    protected bool inSpawnReach = true;
    protected bool isMoving = true;
    protected bool attacking = false;

    //Logic
    protected Transform player;
    private Vector3 spawnPoint;
    private float offensiveDistance = 15f;
    private float spawnPointDistanceLimit = 20f;
    private bool attackInCooldown = false;

    //Basic animations
    protected int idleHash;
    protected int moveHash;
    protected int deathHash;
    protected int parryHash;

    //Basic audio
    [SerializeField] protected AudioClip deathSound;
    [SerializeField] protected AudioSource movementSoundSource;

    //Renderer and materials
    protected Renderer[] entityRenderers;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        entityRenderers = GetComponentsInChildren<Renderer>();
        spawnPoint = transform.position;
    }

    private IEnumerator DamageEffectCoroutine()
    {
        float duration = 0.05f;
        Color damagedColor = new Color(1f, 0.7f, 0.7f);

        foreach (Renderer childRenderer in entityRenderers)
            childRenderer.material.color = damagedColor;

        yield return new WaitForSeconds(duration);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            foreach (Renderer childRenderer in entityRenderers)
                childRenderer.material.color = Color.Lerp(damagedColor, Color.white, elapsedTime / duration);
            yield return null;
        }
    }

    private IEnumerator FadeAwayCoroutine()
    {
        float duration = 5f;
        float elapsedTime = 0f;
        Color fadeAwayColor = new Color(1, 1, 1, 0);

        foreach (Renderer childRenderer in entityRenderers)
                childRenderer.material.shader = Shader.Find("PSX/Vertex Lit Transparent");

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            foreach (Renderer childRenderer in entityRenderers)
                childRenderer.material.color = Color.Lerp(Color.white, fadeAwayColor, elapsedTime / duration);

            yield return null;
        }
    }

    private void Update() 
    {
        float distanceFromPlayer = Vector3.Distance(player.position, transform.position);
        float distanceFromSpawnpoint = Vector3.Distance(transform.position, spawnPoint);

        playerInReach = distanceFromPlayer < offensiveDistance;
        inSpawnReach = distanceFromSpawnpoint < spawnPointDistanceLimit;
        isMoving = false;

        if(!active) return;

        if(attacking)
        {
            if(hitCollider.playerCollided)
            {
                hitCollider.playerCollided = false;
                hitCollider.enabled = false;

                if(CombatHandler.Instance.TakeHit(damage)) 
                    StartCoroutine(Stun());
            }

            return;
        }

        if(stunned || attacking) return;

        if (playerInReach && inSpawnReach)
            offensive = true;
        else 
            offensive = false;

        if(offensive && !goingBackToSpawn)
        {
            bool playerInRange = FollowTarget(player.position);
            if(playerInRange && attackPlayer) Attack();
        }
        else
            goingBackToSpawn = !FollowTarget(spawnPoint);
    }

    public void TakeHit(int damage)
    {
        life -= damage;
        StartCoroutine(DamageEffectCoroutine());

        if(life <= 0)
            Die();
    }

    public virtual void Die()
    {
        StopAllCoroutines();

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Animator>(), 5);
        Destroy(gameObject, 10);

        Effects.Instance.FreezeFrame();
        Effects.Instance.ScreenShake();

        anim.CrossFade(deathHash, 0, 0);
        AudioManager.Instance.PlayOneShot3D(deathSound, gameObject, AudioManager.AudioType.SFX, 1);
        active = false;
        life = 0;

        StartCoroutine(FadeAwayCoroutine());
        foreach(Collider col in GetComponentsInChildren<Collider>())
            Destroy(col);
    }

    public virtual void Attack()
    {
        if(attackInCooldown) return;

        hitCollider.enabled = true;
        LookAtTarget(player.position);
        StartCoroutine(AttackLogic());
    }

    private IEnumerator AttackLogic()
    {
        int choosenAttack = Random.Range(0, attacks.Length);
        int animationHash = Animator.StringToHash(attacks[choosenAttack].attackAnimationName);

        AudioManager.Instance.PlayOneShot3D(attacks[choosenAttack].attackAudio, gameObject, AudioManager.AudioType.SFX, 1);
        anim.CrossFade(animationHash, 0, 0);
        attacking = true;

        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length + 0.5f);

        StartCoroutine(AttackCooldown());
        attacking = false;
    }

    private IEnumerator AttackCooldown()
    {
        attackInCooldown = true; 
        yield return new WaitForSeconds(attackCooldown);
        attackInCooldown = false;
    }

    protected bool FollowTarget(Vector3 targetPos)
    {
        LookAtTarget(targetPos);
        float distance = Vector3.Distance(targetPos, transform.position);
        float distanceToStop = 5f;

        if(distance < distanceToStop)
        {
            anim.CrossFade(idleHash, 0, 0);
            movementSoundSource.Pause();
            isMoving = false;
            return true;
        }
        else
        {
            anim.CrossFade(moveHash, 0, 0);
            if(!movementSoundSource.isPlaying) movementSoundSource.Play();
            movementSoundSource.UnPause();
            rigid.velocity = transform.forward * movementSpeed + transform.up * rigid.velocity.y;
            isMoving = true;
            return false;
        }
    }

    protected IEnumerator Stun()
    {
        anim.CrossFade(parryHash, 0, 0);
        stunned = true;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length * 2);
        stunned = false;
    }

    protected void LookAtTarget(Vector3 targetPos)
    {
        var lookPos = targetPos - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;
    }
}
