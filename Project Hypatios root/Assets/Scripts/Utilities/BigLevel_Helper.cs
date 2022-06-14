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

            foreach(var levelPart in bigLevel.alllevelParts)
            {
                levelPart.sceneObject.SetActive(false);
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

    [Button("Enable All")]
    public void EnableAll()
    { 
        foreach (var levelPart in alllevelParts)
        {
            levelPart.sceneObject.SetActive(true);
        }
    }

}
