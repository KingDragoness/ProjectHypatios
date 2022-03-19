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
            if (eo.key != paradoxLevel.GetValue())
            {
                foreach (var go in eo.allObjects)
                {
                    if (go == null) continue;

                    if (go.gameObject.activeSelf)
                    {
                        go.gameObject.SetActive(false);
                    }
                }

                continue;
            }

            if (paradoxLevel.isPreviewing && eo.dontPreviewThis)
            {
                continue;
            }

            foreach(var go in eo.allObjects)
            {
                if (go == null) continue;

                if (!go.gameObject.activeSelf)
                {
                    go.gameObject.SetActive(true);
                }
            }    
        }

    }

}
