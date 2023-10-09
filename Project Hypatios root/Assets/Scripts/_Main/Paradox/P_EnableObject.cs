using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class P_EnableObject : MonoBehaviour
{

    [System.Serializable]
    public class EnableObjects
    {
        public string key = "";
        public bool dontPreviewThis = false;
        public List<GameObject> allObjects;
    }


    public List<EnableObjects> enableObjects = new List<EnableObjects>();
    [Space(30)]
    public ParadoxLevelScript paradoxLevel;


    private void FixedUpdate()
    {

        foreach(var eo in enableObjects)
        {
            if (Hypatios.Game.DEBUG_UnlockAllParadox == false)
            {
                if (eo.key != paradoxLevel.GetValue())
                {
                    foreach (var go in eo.allObjects)
                    {
                        DeactivateObject(go);
                    }

                }
                else
                {
                    if (paradoxLevel.isPreviewing && eo.dontPreviewThis)
                    {
                        continue;
                    }

                    foreach (var go in eo.allObjects)
                    {
                        EnableObject(go);
                    }
                }
            }
            else
            {
                if (paradoxLevel.isPreviewing && eo.dontPreviewThis)
                {
                    continue;
                }

                if (eo.key != paradoxLevel.buyTargetValue)
                {
                    foreach (var go in eo.allObjects)
                    {
                        DeactivateObject(go);
                    }

                }
                else
                {
                    foreach (var go in eo.allObjects)
                    {
                        EnableObject(go);
                    }
                }
            }
     
        }

    }

    public void DeactivateObject(GameObject go)
    {
        if (go == null) return;

        if (go.gameObject.activeSelf)
        {
            go.gameObject.SetActive(false);
        }
    }
    public void EnableObject(GameObject go)
    {
        if (go == null) return;

        if (!go.gameObject.activeSelf)
        {
            go.gameObject.SetActive(true);
        }
    }
}
