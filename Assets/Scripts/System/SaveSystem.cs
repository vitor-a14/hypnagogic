using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

//This class stores all the relevant variables to save and load into the game
[System.Serializable]
public class Data
{
    public int maxLife;
    public float currentLife;
    public int soulPotions;
    public int goldenSeeds;
    public float[] position;
    public int[] inventoryItemsID;
    public string[] unlockedDoors = new string[0];
    public string[] pickedItems = new string[0];
    public string currentArea;
    public int equippedItemID;

    public void StorePlayerInfo()
    {
        Transform playerTransform = PlayerController.Instance.transform;

        position = new float[3];
        position[0] = playerTransform.position.x;
        position[1] = playerTransform.position.y;
        position[2] = playerTransform.position.z;

        maxLife = PlayerStatus.Instance.maxLife;
        currentLife = PlayerStatus.Instance.currentLife;
        soulPotions = PlayerStatus.Instance.soulPotions;
        goldenSeeds = PlayerStatus.Instance.goldenSeeds;
        currentArea = AreaManager.Instance.currentArea;
    }

    public void StoreInventoryInfo()
    {
        inventoryItemsID = new int[InventoryManager.Instance.items.Count];
        for(int i = 0; i < InventoryManager.Instance.items.Count; i++)
            inventoryItemsID[i] = InventoryManager.Instance.items[i].id;

        if(InventoryManager.Instance.equipedItemData == null)
            equippedItemID = -1;
        else
            equippedItemID = InventoryManager.Instance.equipedItemData.id;
    }

    public void AddUnlockedDoorToInfo(Transform door)
    {
        string[] newUnlockedDoorsList = new string[unlockedDoors.Length + 1];
        for(int i = 0; i < unlockedDoors.Length; i++)
            newUnlockedDoorsList[i] = unlockedDoors[i];

        newUnlockedDoorsList[unlockedDoors.Length] = door.name;
        unlockedDoors = newUnlockedDoorsList;
    }

    public void AddPickedItemToInfo(Transform itemPickup)
    {
        string[] newPickedItemList = new string[pickedItems.Length + 1];
        for(int i = 0; i < pickedItems.Length; i++)
            newPickedItemList[i] = pickedItems[i];

        newPickedItemList[pickedItems.Length] = itemPickup.name;
        pickedItems = newPickedItemList;
    }
}

//This class is responsible to save and load the class above and distribute among the others classes
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    public Data gameData;

    private void Start() 
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.Log("Instance is already set, something is wrong");

        //LoadGame();
    }

    public void SaveGame()
    {
        gameData.StorePlayerInfo();
        gameData.StoreInventoryInfo();

        string path = Application.persistentDataPath + "/data.save";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();
        Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        //Load file
        string path = Application.persistentDataPath + "/data.save";

        if(!File.Exists(path))
        {
            Debug.LogError("Save file not found");
            return;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        gameData = formatter.Deserialize(stream) as Data;
        stream.Close();

        //Load variables into the game

        //Player status and position
        PlayerController.Instance.transform.position = new Vector3(gameData.position[0], gameData.position[1], gameData.position[2]);
        PlayerStatus.Instance.maxLife = gameData.maxLife;
        PlayerStatus.Instance.currentLife = gameData.currentLife;
        PlayerStatus.Instance.soulPotions = gameData.soulPotions;
        PlayerStatus.Instance.goldenSeeds = gameData.goldenSeeds;
        AreaManager.Instance.currentArea = gameData.currentArea;

        //Inventory data
        Item[] allItems = (Item[])Resources.FindObjectsOfTypeAll(typeof(Item));
        List<Item> inventory = new List<Item>();

        foreach(Item item in allItems)
        {
            foreach(int itemId in gameData.inventoryItemsID)
            {
                if(item.id == itemId)
                    inventory.Add(item);
            }

            if(item.id == gameData.equippedItemID)
                InventoryManager.Instance.EquipItem(item);
        }

        InventoryManager.Instance.items = inventory;
        Debug.Log("Game loaded!");
    }
}
