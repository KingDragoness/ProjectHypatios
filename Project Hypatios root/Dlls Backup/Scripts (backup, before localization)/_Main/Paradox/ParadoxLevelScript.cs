using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ParadoxLevelScript : MonoBehaviour
{

    public bool isRequireTrivia = false;
    [ShowIf("isRequireTrivia", true)]
    public Trivia triviaRequired;
    public ParadoxEntity paradoxEntity;
    [Space]
    public GameObject virtualCamera;
    public List<GameObject> yellowObjectPreview;
    [Space]
    public int soulPrice = 10;
    public string buyTargetValue = "1";
    public string paradoxName = "Secret Corridor";
    [TextArea(3,4)]
    public string description = "This will unlock a new area to explore some new places.";

    [Space]
    public bool isPreviewing = false;

    private void Start()
    {
        virtualCamera.gameObject.SetActive(false);
        RegisterThis();
    }

    public string GetValue()
    {
        if (isPreviewing)
        {
            return buyTargetValue;
        }
        else
        {
            return paradoxEntity.value;
        }
    }

    public bool IsTriviaFulfilled()
    {
        return Hypatios.Game.Check_TriviaCompleted(triviaRequired);
    }

    private void RegisterThis()
    {
        var collectionEntity = Hypatios.Game.paradoxEntities;
        var entity = collectionEntity.Find(x => x.ID == paradoxEntity.ID);

        if (entity == null)
        {
            entity = paradoxEntity;
            collectionEntity.Add(entity);
            Debug.Log($"{entity.ID} is null");
        }
        else
        {
            paradoxEntity = entity;
            Debug.Log($"{entity.ID} {entity.value}");
        }

    }

    [ContextMenu("Debug_EnableThis")]
    public void Debug_EnableThis()
    {
    }

    private void Update()
    {
        if (isPreviewing)
        {
            foreach(var go in yellowObjectPreview)
            {
                if (go == null) continue;

                go.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var go in yellowObjectPreview)
            {
                if (go == null) continue;

                go.gameObject.SetActive(false);
            }
        }
    }

}
