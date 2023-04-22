using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public string areaName;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
            AreaManager.Instance.ChangeArea(areaName);
    }

}
