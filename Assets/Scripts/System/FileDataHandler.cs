using UnityEngine;
using System.IO;
using System;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public Data Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        Data loadedData = null;
        if(File.Exists(fullPath))
        {
            try 
            {
                string dataToLoad = "";

                //Load data
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //desirialize
                loadedData = JsonUtility.FromJson<Data>(dataToLoad);
            } 
            catch(Exception e) 
            {
                Debug.LogError("Error while trying to load data in path: " + fullPath + "\n " + e);
            }  
        }

        return loadedData;
    }

    public void Save(Data gameData)
    {
        //Combine the path based on the OS
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try 
        {
            //Create directory
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize data
            string dataToStore = JsonUtility.ToJson(gameData, true);

            //Write file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        } 
        catch(Exception e) 
        {
            Debug.LogError("Error while trying to save data in path: " + fullPath + "\n " + e);
        }
    }
}
