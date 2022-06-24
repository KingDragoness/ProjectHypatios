using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class MechizPatternRobot : MonoBehaviour
{

    [FoldoutGroup("Events")] public UnityEvent OnHalfHP;
    [FoldoutGroup("Events")] public UnityEvent OnQuarterHP;

    public MechizMonsterRobot mechizRobot;
    public MechizDroneMonster droneMonster;
    public List<Transform> sawingPositions = new List<Transform>();
    public List<Transform> flyingPositions = new List<Transform>();
    public List<Transform> droneSpawnPositions = new List<Transform>();

    private List<MechizDroneMonster> allMechizSpawns = new List<MechizDroneMonster>();

    public Transform GetRandomSawPosition()
    {
        return sawingPositions[Random.Range(0, sawingPositions.Count)];
    }

    public Transform GetDronePosition()
    {
        return droneSpawnPositions[Random.Range(0, droneSpawnPositions.Count)];
    }

    public Transform GetRandomFlyingPos()
    {
        return flyingPositions[Random.Range(0, flyingPositions.Count)];

    }

    private float timer_ChanceSpawnDrone = 4f;
    private bool PercentageHalf = false;
    private bool Percentage20 = false;

    private void Update()
    {
        float percentageHP = mechizRobot.hitpoint / mechizRobot.maxHitpoint;

        if (percentageHP < 0.6f)
        {
            timer_ChanceSpawnDrone -= Time.deltaTime;

            if (timer_ChanceSpawnDrone < 0)
            {
                float chance1 = Random.Range(0f, 1f);

                if (chance1 < 0.4f)
                {
                    SpawnDrone(droneMonster.gameObject);
                }
                else
                {

                }

                timer_ChanceSpawnDrone = 4f;
            }
        }

        if (percentageHP < 0.5f)
        {
            RunEvent_HalfHP();
            PercentageHalf = true;
        }

        if (percentageHP < 0.2f)
        {
            RunEvent_QuarterHP();
            Percentage20 = true;
        }
    }

    private void RunEvent_HalfHP()
    {
        if (!PercentageHalf)
        {
            OnHalfHP?.Invoke();
        }
    }

    private void RunEvent_QuarterHP()
    {
        if (!Percentage20)
        {
            OnQuarterHP?.Invoke();
        }
    }

    private void SpawnDrone(GameObject monster)
    {
        allMechizSpawns.RemoveAll(x => x == null);

        //Limit 3
        if (allMechizSpawns.Count > 3) { return; }

        var prefabDrone = Instantiate(monster, GetDronePosition().position, Quaternion.identity).GetComponent<MechizDroneMonster>();
        allMechizSpawns.Add(prefabDrone);
        prefabDrone.gameObject.SetActive(true);
    }

}
