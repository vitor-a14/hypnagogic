using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public string weaponType;
    public int damage;
    public float velocity;
    public Entity target;
    public AudioClip hitAudio;

    //Send the weapon statistics for the combat handler class, this class is stored in the pivot in the camera
    void Start()
    {
        CombatHandler.Instance.SetWeapon(this);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Entity" && CombatHandler.Instance.attacking)
        {
            Debug.Log("hit");
            Effects.Instance.FreezeFrame();
            Effects.Instance.ScreenShake();
            AudioManager.Instance.PlayOneShot3D(hitAudio, gameObject, AudioManager.AudioType.SFX, 1);
            target = other.GetComponent<Entity>();
        }
    }
}
