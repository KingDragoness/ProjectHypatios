using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInstantiateScript : MonoBehaviour
{

    public GameObject prefab1;

    public GameObject SpawnObject()
    {
        GameObject newPrefab = Instantiate(prefab1);
        return newPrefab;
    }

    public GameObject SpawnObject(GameObject _prefab)
    {
        GameObject newPrefab = Instantiate(_prefab);
        return newPrefab;
    }

}
