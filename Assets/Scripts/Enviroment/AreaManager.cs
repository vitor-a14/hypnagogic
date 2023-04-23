using UnityEngine;
using TMPro;

public class AreaManager : MonoBehaviour, IDataPersistance
{
    public static AreaManager Instance { get; private set; }
    public string currentArea; 

    [SerializeField] private TMP_Text areaUITitle;

    void Awake()
    {
        //Set a global static instance for weapons to acess
        if(Instance == null)
            Instance = this;
        else
            Debug.LogError(this.name + " is trying to set a Instance, but seems like a instance is already attributed.");
    }

    public void Save(ref Data gameData)
    {
        gameData.currentArea = currentArea;
    }

    public void Load(Data gameData)
    {
        currentArea = gameData.currentArea;
    }

    public void ChangeArea(string areaName)
    {
        if(areaName == currentArea) return;

        areaUITitle.text = areaName;
        HUDManager.Instance.FadeInAndOut(areaUITitle.gameObject, 4, 6f);
        currentArea = areaName;
    }
}
