using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InstantiateRandomObject : MonoBehaviour
{

    [ShowIf("useRandomSpawn")] public RandomSpawnArea randomPositioner;
    [HideIf("useRandomSpawn")] public List<Transform> SpawnList = new List<Transform>();
    public List<GameObject> prefabs;
    public bool useRandomSpawn = false;

    public GameObject SpawnThing()
    {
        var go = prefabs[Random.Range(0, prefabs.Count)];

        if (useRandomSpawn)
        {
            var go1 = Instantiate(go, randomPositioner.GetAnyPositionInsideBox(), go.transform.rotation);
            IntializedSpawn(go1);
            return go1;
        }
        else
        {
            var t = SpawnList[Random.Range(0, SpawnList.Count)];
            var go1 = Instantiate(go, t.transform.position, t.rotation);
            IntializedSpawn(go1);
            return go1;
        }
    }

    public void SpawnThing(GameObject prefab1)
    {
        if (useRandomSpawn)
        {
            var go1 = Instantiate(prefab1, randomPositioner.GetAnyPositionInsideBox(), prefab1.transform.rotation);
            IntializedSpawn(go1);

        }
        else
        {
            var t = SpawnList[Random.Range(0, SpawnList.Count)];
            var go1 = Instantiate(prefab1, t.transform.position, t.rotation);
            IntializedSpawn(go1);
        }
    }

    private void IntializedSpawn(GameObject go1)
    {
        go1.SetActive(true);
    }

}
