using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance 
{
    void Save(ref Data gameData);
    void Load(Data gameData);
}
