using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public string weaponType;
    public int damage;
    public float velocity;
    public AudioClip hitAudio;
    public AudioClip drawWeaponAudio;
    [HideInInspector] public Entity target;

    //Send the weapon statistics for the combat handler class, this class is stored in the pivot in the camera
    void Start()
    {
        CombatHandler.Instance.SetWeapon(this);
        AudioManager.Instance.PlayOneShot2D(drawWeaponAudio, gameObject, AudioManager.AudioType.SFX, 1);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!CombatHandler.Instance.attacking) return;

        target = other.transform.root.GetComponent<Entity>();
    }

    private void OnTriggerExit(Collider other) 
    {
        target = null;
    }
}
