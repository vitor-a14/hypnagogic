using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public string weaponType;
    public int damage;
    public float velocity;
    public AudioClip hitAudio;
    public AudioClip drawWeaponAudio;
    [HideInInspector] public BoxCollider weaponColider;

    //Send the weapon statistics for the combat handler class, this class is stored in the pivot in the camera
    void Start()
    {
        CombatHandler.Instance.SetWeapon(this);
        AudioManager.Instance.PlayOneShot2D(drawWeaponAudio, gameObject, AudioManager.AudioType.SFX, 1);
        weaponColider = GetComponent<BoxCollider>();
        weaponColider.enabled = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Enemy")
        {
            AudioManager.Instance.PlayOneShot3D(hitAudio, gameObject, AudioManager.AudioType.SFX, 1);
            other.transform.root.GetComponent<Entity>().TakeHit(damage);
        }
    }
}
