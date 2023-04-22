using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public string description;
    public bool isEquippable;
    public GameObject equippableInstance;
    public GameObject model;
}
