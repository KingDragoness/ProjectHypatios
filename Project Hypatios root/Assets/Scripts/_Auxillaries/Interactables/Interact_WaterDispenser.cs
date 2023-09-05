using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_WaterDispenser : MonoBehaviour
{

    public enum Stage
    {
        Empty,
        EmptyCup,
        ColdWater,
        HotWater
    }

    public GameObject prefab_EmptyCup;
    public GameObject prefab_ColdWater;
    public GameObject prefab_HotWater;
    public Stage currentStage = Stage.Empty;


    private void Start()
    {
        RefreshCups();
    }

    public void PutCup()
    {
        if (currentStage != Stage.Empty)
        {
            RefreshCups();
            return;
        }
        currentStage = Stage.EmptyCup;
        RefreshCups();
    }

    public void FillCup(bool isHot)
    {
        if (currentStage == Stage.Empty)
        {
            DeadDialogue.PromptNotifyMessage_Mod("There's no cup to fill water.", 4f);
            return;
        }

        if (isHot)
        {
            currentStage = Stage.HotWater;
        }
        else
        {
            currentStage = Stage.ColdWater;
        }

        RefreshCups();
    }

    public void TakeCup()
    {
        currentStage = Stage.Empty;
        RefreshCups();
    }

    public void RefreshCups()
    {
        if (currentStage == Stage.EmptyCup)
        {
            prefab_EmptyCup.gameObject.SetActive(true);
        }
        else
        {
            prefab_EmptyCup.gameObject.SetActive(false);
        }

        if (currentStage == Stage.ColdWater)
        {
            prefab_ColdWater.gameObject.SetActive(true);
        }
        else
        {
            prefab_ColdWater.gameObject.SetActive(false);
        }

        if (currentStage == Stage.HotWater)
        {
            prefab_HotWater.gameObject.SetActive(true);
        }
        else
        {
            prefab_HotWater.gameObject.SetActive(false);
        }
    }

}
