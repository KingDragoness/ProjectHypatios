using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpawnPlayerDebug : MonoBehaviour
{
    
    [Button("Spawn")]
    public void SpawnPlayer()
    {
        var player = FindObjectOfType<characterScript>();
        player.transform.position = transform.position;
    }

}
