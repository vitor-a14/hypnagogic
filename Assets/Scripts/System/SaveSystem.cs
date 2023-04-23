using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//This class stores all the relevant variables to save and load into the game
[System.Serializable]
public class Data
{
    public int maxLife;
    public float currentLife;
    public int soulPotions;
    public int goldenSeeds;
    public float[] playerPosition;
    public int[] inventoryItemsID;
    public string currentArea;
    public int equippedItemID;
    public string dialogueVariables;

    public SerializableDictionary<string, bool> unlockedDoors;
    public SerializableDictionary<string, bool> collectedItems;

    public Data()
    {
        unlockedDoors = new SerializableDictionary<string, bool>();
        collectedItems = new SerializableDictionary<string, bool>();
    }
}

//This class is responsible to save and load the class above and distribute among the others classes
public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;
    [SerializeField] private string fileName;
    public Data gameData; //where the actual game info is stored

    private void Awake() 
    {
        if(Instance == null)
            Instance = this;
        else
            Debug.Log("Instance is already set, something is wrong");
    }

    private void Start() 
    {
        this.gameData = new Data();
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        LoadGame();
    }

    public void SaveGame()
    {
        foreach(IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.Save(ref gameData);
        }

        //Save file
        dataHandler.Save(gameData);
    }

    public void LoadGame()
    {
        //Load file
        gameData = dataHandler.Load();

        if(gameData == null)
        {
            Debug.Log("No save files foud, initializing the game without saves");
            return;
        }

        foreach(IDataPersistance dataPersistanceObject in dataPersistanceObjects)
        {
            dataPersistanceObject.Load(gameData);
        }
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistanceObjects);
    } 
}
