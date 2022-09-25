using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class FW_ControlPoint : MonoBehaviour
{

    /// <summary>
    /// 0 = Finalbase!
    /// 1 = CP1
    /// 2 = CP2
    /// </summary>
    public int CPNumber = 1;
    public RandomSpawnArea areaCP;
    public bool isCaptured = false;

    [ProgressBar(0,1f)]
    public float captureProgress = 0f;

    private int _currentInvadersInArea = 0;
    private characterScript characterScript;

    private void Start()
    {
        characterScript = FindObjectOfType<characterScript>();
    }

    public int CurrentInvadersInArea
    {
        get { return _currentInvadersInArea; }
    }

    private void Update()
    {
        var listInvaders = Chamber_Level7.instance.AllUnits.Where(x => x.Alliance == FW_Alliance.INVADER);
        List<Transform> allUnits = new List<Transform>();

        foreach(var unit in listInvaders)
        {
            allUnits.Add(unit.transform);
        }

        allUnits.Add(characterScript.transform);

        int i = 0;

        foreach(var invader in allUnits)
        {
            if (areaCP.IsInsideOcclusionBox(invader.transform.position))
            {
                i++;
            }
        }

        _currentInvadersInArea = i;

        if (_currentInvadersInArea > 0)
        {
            captureProgress += Time.deltaTime * _currentInvadersInArea * 0.02f;
        }
        else
        {
            captureProgress -= Time.deltaTime * 0.08f;
        }
        captureProgress = Mathf.Clamp(captureProgress, 0f, 1f);
    }

    public void CaptureCP()
    {
        isCaptured = true;
    }

}
