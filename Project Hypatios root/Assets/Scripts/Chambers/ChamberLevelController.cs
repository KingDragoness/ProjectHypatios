using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ChamberLevelController : MonoBehaviour
{

    public ChamberLevel chamberObject;

    public static ChamberLevelController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SaveStartLevel();
    }

    private void SaveStartLevel()
    {
        if (chamberObject == null) return;

        var chamberSave = GetSaveData();

        if (chamberSave == null)
        {
            chamberSave = new HypatiosSave.ChamberDataSave(chamberObject.GetID());
            Hypatios.Game.Game_ChamberSaves.Add(chamberSave);
        }

        chamberSave.lastRunVisit = Hypatios.Game.TotalRuns;
        chamberSave.timesVisited++;
    }

    private HypatiosSave.ChamberDataSave GetSaveData()
    {
        var chamberSave = Hypatios.Game.Game_ChamberSaves.Find(x => x.ID == chamberObject.GetID());
        return chamberSave;
    }

    public static HypatiosSave.ChamberDataSave GetSaveData(ChamberLevel chamberLevel)
    {
        var chamberSave = Hypatios.Game.Game_ChamberSaves.Find(x => x.ID == chamberLevel.GetID());
        return chamberSave;
    }

    public void ChamberCompleted()
    {
        var chamberSave = GetSaveData();
        if (chamberSave == null)
        {
            Debug.LogError("Chamber save data is missing!");
            return;
        }

        chamberSave.timesCompleted++;
    }

}
