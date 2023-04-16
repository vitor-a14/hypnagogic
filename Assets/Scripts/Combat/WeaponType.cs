using UnityEngine;

public class WeaponType : MonoBehaviour
{
    public string weaponType;
    public int damage;
    public float velocity;

    //Send the weapon statistics for the combat handler class, this class is stored in the pivot in the camera
    void Start()
    {
        CombatHandler.Instance.SetWeapon(this);
    }
}
