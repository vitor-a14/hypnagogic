using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public string weaponType;
    public int damage;
    public float velocity;
    public Entity target;
    public AudioClip hitAudio;
    public AudioClip drawWeaponAudio;

    //Send the weapon statistics for the combat handler class, this class is stored in the pivot in the camera
    void Start()
    {
        CombatHandler.Instance.SetWeapon(this);
        AudioManager.Instance.PlayOneShot2D(drawWeaponAudio, gameObject, AudioManager.AudioType.SFX, 1);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Entity" && CombatHandler.Instance.attacking)
        {
            Effects.Instance.ScreenShake();
            AudioManager.Instance.PlayOneShot3D(hitAudio, gameObject, AudioManager.AudioType.SFX, 1);
            target = other.transform.root.GetComponent<Entity>();
            target.TakeHit(damage);
        }
    }
}
