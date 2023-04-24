using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInstantiateScript : MonoBehaviour
{

    public GameObject prefab1;

    public void SpawnObject1()
    {
        var go1 = SpawnObject();
        go1.transform.position = transform.position;
        go1.transform.rotation = transform.rotation;
        go1.SetActive(true);
    }

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
