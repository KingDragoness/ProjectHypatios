using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MicrobotsControllerSystem : MonoBehaviour
{

    public List<EnemyTest_Swarm> AllSwarmDrones = new List<EnemyTest_Swarm>();
    [SerializeField] [ReadOnly] private List<EnemyTest_Swarm> _currentSimulatedBots = new List<EnemyTest_Swarm>();
    public LayerMask layer_SwarmDetect;
    public ModularTurretGun modularGun;
    public float CooldownUpdateAI = 0.1f;
    [Tooltip("Note: affected by update AI tick.")] public float CooldownShoot = 0.06f;
    public float AvoidanceDistance = 3f;
    [Range(0f,1f)] public float ChanceDroneShoot = 0.3f;
    [Range(1, 9)] public int MaxDroneShootPerTick = 3;
    [Tooltip("Amount of drones simulated per tick. i.e: 5 means maximum of 50 drones simulated per second (Tick = 0.1s).")] [Range(1,20)] public int PerTick_SimulateDrones = 5;

    private float _timerUpdate = 0.1f;
    private float _timerShooting = 0.05f;
    private int _tick = 0;
    private int _currentIndex = 0;

    public static MicrobotsControllerSystem instance;


    private void Awake()
    {
        instance = this;
        AllSwarmDrones.Clear();
    }

    #region AI Cycling

    private bool IsCurrentIndexValid()
    {
        int maximumIndex = Mathf.CeilToInt((float)AllSwarmDrones.Count / (float)PerTick_SimulateDrones); //44 drones / 5 pertick = 8,8 => 9 indexes

        //_curentindex is 8 valid, curentindex == 9 is not
        if (maximumIndex <= _currentIndex)
        {
            return false;
        }

        return true;
    }

    private List<EnemyTest_Swarm> GetCurrentTickDrones()
    {
        List<EnemyTest_Swarm> allSwarmBots = new List<EnemyTest_Swarm>();
        if (IsCurrentIndexValid() == false) return allSwarmBots;
        int i_low = _currentIndex * PerTick_SimulateDrones; //let's say start index: 8 (8 x 5 = 40)
        int i_high = (_currentIndex + 1) * PerTick_SimulateDrones; // 9 (9 x 5 = 45)

        if (i_high >= AllSwarmDrones.Count) i_high = AllSwarmDrones.Count;
        for (int x = i_low; x < i_high; x++)
        {
            allSwarmBots.Add(AllSwarmDrones[x]);
        }

        return allSwarmBots;
    }

    #endregion

    private void Update()
    {

        if (_timerShooting > 0f)
        {
            _timerShooting -= Time.deltaTime;
        }
       
        if (_timerUpdate > 0f)
        {
            _timerUpdate -= Time.deltaTime;
            return;
        }

        if (_timerShooting <= 0)
        {
            UpdateShoot();
            _timerShooting = CooldownShoot;
        }
        _timerUpdate = CooldownUpdateAI;
        _tick++;

        if (IsCurrentIndexValid())
        {
            _currentSimulatedBots = GetCurrentTickDrones();
            _currentIndex++;
        }
        else
        {
            _currentIndex = 0;
        }

        UpdateFlock();
    }

    private void UpdateFlock()
    {
        foreach(var drone in _currentSimulatedBots)
        {
            if (drone == null) continue;
            if (drone.currentTarget == null) continue;
            drone.AI_Detection();
            if (drone.LastTimeSeenPlayer > 0f)
            {
                drone.avoidanceLeft = false;
                drone.avoidanceRight = false;
            }
            else if (drone.HitDetection.collider != null && drone.currentTarget != null && drone.HitDetection.distance < AvoidanceDistance)
            {
                var dir = (drone.transform.position - drone.currentTarget.transform.position).normalized;
                float dirFloat = IsopatiosUtility.AngleDir(drone.transform.forward, dir, drone.transform.up);

                if (dirFloat < -0.1f)
                    drone.avoidanceLeft = true;
                if (dirFloat > 0.1f)
                    drone.avoidanceRight = true;
            }
        }
    }

    private void UpdateShoot()
    {
        int i = 0;
        foreach (var drone in AllSwarmDrones)
        {
            if (drone == null) continue;
            if (drone.currentTarget == null) continue;
            if (drone.canLookAtTarget == false) continue;
            float c1 = Random.Range(0f, 1f);
            if (ChanceDroneShoot < c1) continue;

            if (i > MaxDroneShootPerTick) break;
            modularGun.transform.position = drone.transform.position;
            modularGun.transform.rotation = drone.EyeLocation.transform.rotation;
            modularGun.FireTurret(drone.transform.position, drone.EyeLocation.transform.forward, 1000f);

            i++;
        }
    }



}
