using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kThanidMissile : MonoBehaviour
{


    public ExplosionAreaEffect prefab1;

    public void CreateExplosion()
    {
        var go1 = SpawnObject();
        go1.transform.position = transform.position; //fucking set transform position not synchronous
        go1.transform.rotation = transform.rotation;
        go1.SetActive(true);
    }

    public GameObject SpawnObject()
    {
        GameObject newPrefab = Instantiate(prefab1).gameObject;
        return newPrefab;
    }

}
