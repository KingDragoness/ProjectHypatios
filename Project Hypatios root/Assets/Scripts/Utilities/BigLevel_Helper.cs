using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AI;
#endif

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
    public List<GameObject> allNavigationStaticGOs = new List<GameObject>();
    [Space]
    public bool enableAllByStart = false;

    [FoldoutGroup("Navigations")]
    public LayerMask navmeshExcludeLayer;

    private void Start()
    {
        if (enableAllByStart)
            EnableAll();
    }

    [FoldoutGroup("Quick tools")]
    [Button("Set statics")]
    public void SetNavigationStatics()
    {
    #if UNITY_EDITOR

        foreach (var go in allNavigationStaticGOs)
        {
            var listGOs = go.AllChilds();

            foreach(var go1 in listGOs)
            {
                if (navmeshExcludeLayer.Contains(go1.layer)) continue;

                StaticEditorFlags flag = StaticEditorFlags.NavigationStatic | StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic;
                GameObjectUtility.SetStaticEditorFlags(go1, flag);

            }
        }
    #endif

    }

    [FoldoutGroup("Navigations")]
    [Button("Bake Navmesh")]
    public void BakeNavmesh()
    {
#if UNITY_EDITOR
        EnableAll();

        NavMeshBuilder.BuildNavMesh();
#endif

    }

    [FoldoutGroup("Quick tools")]
    [Button("Bake Occlusion")]
    public void BakeOcclusion()
    {
#if UNITY_EDITOR
        EnableAll();

        StaticOcclusionCulling.GenerateInBackground();
    #endif

    }

    [FoldoutGroup("Quick tools")]
    [Button("Enable All")]
    public void EnableAll()
    { 
        foreach (var levelPart in alllevelParts)
        {
            levelPart.sceneObject.SetActive(true);
        }
    }

    [FoldoutGroup("Quick tools")]
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
