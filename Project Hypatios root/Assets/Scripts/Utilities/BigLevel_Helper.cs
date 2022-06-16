using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BigLevel_Helper : MonoBehaviour
{
    
    [System.Serializable]
    public class LevelPart
    {
        //public string namePart = "Stage 001";

        [HideLabel()]
        [HorizontalGroup("Scene Object")]
        public GameObject sceneObject;

        [HorizontalGroup("Scene Object")]
        [Button("Show only")]
        public void EnableThisOnly()
        {
            BigLevel_Helper bigLevel = FindObjectOfType<BigLevel_Helper>();
            List<Transform> allParentsAssociated = new List<Transform>();

            foreach(var levelPart in bigLevel.alllevelParts)
            {
                levelPart.sceneObject.SetActive(false);

                if (sceneObject.IsParentOf(levelPart.sceneObject))
                {
                    allParentsAssociated.Add(levelPart.sceneObject.transform);
                }
            }

            foreach (var parent in allParentsAssociated)
            {
                parent.gameObject.SetActive(true);
            }

            sceneObject.SetActive(true);
        }

        [HorizontalGroup("Scene Object")]
        [Button("Show")]
        public void ShowThis()
        {
            BigLevel_Helper bigLevel = FindObjectOfType<BigLevel_Helper>();

            foreach (var levelPart in bigLevel.alllevelParts)
            {
                //levelPart.sceneObject.SetActive(false);
            }

            sceneObject.SetActive(true);
        }

        [HorizontalGroup("Scene Object")]
        [Button("Hide")]
        public void HideThis()
        {
            BigLevel_Helper bigLevel = FindObjectOfType<BigLevel_Helper>();

            foreach (var levelPart in bigLevel.alllevelParts)
            {
                //levelPart.sceneObject.SetActive(false);
            }

            sceneObject.SetActive(false);
        }

    }

    [TableList]
    public List<LevelPart> alllevelParts = new List<LevelPart>();
    [Space]
    public bool enableAllByStart = false;

    private void Start()
    {
        if (enableAllByStart)
            EnableAll();
    }

    [TabGroup("Quick tools")]
    [Button("Enable All")]
    public void EnableAll()
    { 
        foreach (var levelPart in alllevelParts)
        {
            levelPart.sceneObject.SetActive(true);
        }
    }

    [TabGroup("Quick tools")]
    [Button("Close ATVRegion Gizmos")]
    public void DisableGizmosATV()
    {
        var ActivatorRegions = FindObjectsOfType<ActivatorRegion>();

        foreach (var atvreg in ActivatorRegions)
        {
            atvreg.DEBUG_DrawGizmos = false;
        }
    }

}
