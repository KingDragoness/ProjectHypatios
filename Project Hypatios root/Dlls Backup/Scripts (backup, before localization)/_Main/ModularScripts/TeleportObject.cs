using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObject : MonoBehaviour
{

    public Transform destination;
    public GameObject objectToTeleport;

    
    public void Teleport()
    {
        objectToTeleport.transform.position = destination.transform.position;
    }

}
