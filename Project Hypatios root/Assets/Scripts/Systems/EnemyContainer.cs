using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;


public class EnemyContainer : MonoBehaviour
{

    //This is absolutely retarded
    [System.Serializable]
    public class PathReportToken
    {
        public bool success = false;
        public string objectName = "";
        public float distance = 0;
        public Vector3 result = new Vector3();
        public NavMeshPath navMeshPath;
    }

    [ReadOnly] [ShowInInspector] private List<EnemyScript> AllEnemies = new List<EnemyScript>();
    public LayerMask baseDetectionLayer;
    public LayerMask baseSolidLayer; //non transparent
    public System.Action<EnemyScript, DamageToken> OnEnemyDied;

    [FoldoutGroup("Statistics")] public BaseStatValue stat_kill;
    [FoldoutGroup("Statistics")] public BaseStatValue stat_kill_melee;
    [FoldoutGroup("Debug")] public Transform debug_pathTarget;
    [FoldoutGroup("Debug")] public bool DEBUG_TestCheckUnit;

    private bool _isPlayerInNavMesh = false;
    private bool _isPlayerReachable = false;

    /// <summary>
    /// Don't use this in FortWar
    /// </summary>
    public bool IsPlayerInNavMesh { get => _isPlayerInNavMesh; }
    public bool IsPlayerReachable { get => _isPlayerReachable; }

    private void Awake()
    {
        OnEnemyDied += OnEnemyDieEvent;
    }

    #region Utility events
    //Error in here means missing _lastdamagetoken!
    public void OnEnemyDieEvent(EnemyScript enemy, DamageToken token)
    {
        if (token == null)
        {
            Debug.LogError("Damage token is missing!");
            return;
        }
        if (token.origin == DamageToken.DamageOrigin.Player &&
            enemy.Stats.MainAlliance != Alliance.Player)
        {
            Hypatios.Game.Increment_PlayerStat(stat_kill);

            if (token.damageType == DamageToken.DamageType.PlayerPunch)
                Hypatios.Game.Increment_PlayerStat(stat_kill_melee);

        }

    }

    public void RegisterEnemy(EnemyScript enemy)
    {
        AllEnemies.Add(enemy);
        AllEnemies.RemoveAll(x => x == null);
    }

    public void DeregisterEnemy(EnemyScript enemy)
    {
        AllEnemies.Remove(enemy);
        AllEnemies.RemoveAll(x => x == null);
    }

    public int CountMyEnemies(Alliance alliance)
    {
        return AllEnemies.FindAll(x => x.Stats.MainAlliance != alliance).Count;
    }

    public int CountMyAlly(Alliance alliance)
    {
        return AllEnemies.FindAll(x => x.Stats.MainAlliance == alliance).Count;
    }

    public int CountEnemyOfType(EnemyScript stat)
    {
        return AllEnemies.FindAll(x => x.EnemyName == stat.EnemyName).Count;
    }

    #endregion

    private float _cooldownPlayerNavMeshValid = 0.1f;

    private void Update()
    {
        if (AllEnemies.Count == 0) return;

        _cooldownPlayerNavMeshValid -= Time.deltaTime;

        if (_cooldownPlayerNavMeshValid < 0f)
        {
            var listNonplayer = AllEnemies.FindAll(x => x.Stats.MainAlliance != Alliance.Player);
            if (listNonplayer.Count() > 0) CheckCalculate(listNonplayer[Random.Range(0, listNonplayer.Count)]);
            _cooldownPlayerNavMeshValid = 0.1f;
        }

        if (DEBUG_TestCheckUnit) DEBUG_TestPathFindUnit();
    }

    //Optimized for enemies around 20-50
    #region Horde systems

    private Entity[] horde_targetFocus = new Entity[3];
    private float horde_lastTimeCheck = 0f;

    private void DEBUG_TestPathFindUnit()
    {
        stupidTokens = new PathReportToken[tempList_NearestEnemy.Count];    
        int i = 0;

        foreach(var x in tempList_NearestEnemy)
        {
            var newtoken = GetDistanceByNavMesh(x.transform, GetRandomEnemyEntity(Alliance.Mobius));
            newtoken.objectName = $"{x.gameObject.name}";
            if (newtoken.navMeshPath != null) newtoken.distance = newtoken.navMeshPath.GetPathLength();
            if (newtoken.success == false) newtoken.distance = 999f;
            stupidTokens[i] = newtoken;
            float dist = newtoken.distance;
            i++;
            Debug.Log($"{x.gameObject.name} DIST: {dist}");
        }

        currentTOKEN = stupidTokens[0];
        debug_pathTarget.transform.position = stupidTokens[0].result;
    }

    public Entity Horde_GetPlayerAlly(Alliance alliance)
    {
        if (horde_lastTimeCheck + 1f < Time.timeSinceLevelLoad)
        {
            horde_lastTimeCheck = Time.timeSinceLevelLoad;
            //sampling
            int i = 0;
            int totalHorde = CountMyAlly(alliance);
            if (totalHorde >= 3) totalHorde = 3;
            while (i < totalHorde)
            {
                EnemyScript _entity = GetRandomEnemyEntity(alliance) as EnemyScript;
                if (_entity != null) horde_targetFocus[i] = Hypatios.Enemy.FindEnemyEntity(alliance, _entity, chanceSelectAlly: 1f, distanceNavmesh: true);            
                i++;
            }
        }

        var listFocuses = horde_targetFocus.ToList(); listFocuses.RemoveAll(x => x == null);

        if (listFocuses.Count == 0)
            return null;

        return listFocuses[Random.Range(0, listFocuses.Count-1)];
    }

    #endregion


    #region Pathing
    private void CheckCalculate(EnemyScript enemyScript)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        if (enemyScript == null) return;
        var agent = enemyScript.GetComponent<NavMeshAgent>();
        NavMeshHit hit;

        if (agent == null) return;
        if (agent.enabled == false) return;
        if (enemyScript.gameObject.activeInHierarchy == false) return;
        if (enemyScript.Stats.IsDead == true) return;

        Vector3 groundPlayer = Hypatios.Player.transform.position;
        groundPlayer.y -= 1f;

        if (agent.CalculatePath(Hypatios.Player.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            _isPlayerInNavMesh = true;
            _isPlayerReachable = true;
        }
        else if (NavMesh.SamplePosition(groundPlayer, out hit, 2f, NavMesh.AllAreas))
        {
            _isPlayerInNavMesh = true;

            if (agent.CalculatePath(hit.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                _isPlayerReachable = true;
            else
                _isPlayerReachable = false;
        }
        else
        {
            _isPlayerInNavMesh = false;
            _isPlayerReachable = false;
        }

    }

    public PathReportToken GetDistanceByNavMesh(Transform currentTarget, Entity entityScript = null)
    {
        NavMeshHit hit;
        NavMeshPath navMeshPath = new NavMeshPath();
        var agent = entityScript.GetComponent<NavMeshAgent>();
        PathReportToken token = new PathReportToken();
        token.result = new Vector3(999, -999, 999);

        if (agent == null)
            return token;


        //failed due to being blocked by its navmeshobstacle
        if (agent.CalculatePath(currentTarget.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            //var length = navMeshPath.GetPathLength();
            token.success = true;
            token.result = currentTarget.transform.position;
            token.navMeshPath = navMeshPath;
            return token;
        }
        else if (navMeshPath.status == NavMeshPathStatus.PathPartial)
        {
            //var length = navMeshPath.GetPathLength();
            token.success = false;
            token.result = currentTarget.transform.position;
            token.navMeshPath = navMeshPath;
            return token;
        }
        else if (navMeshPath.status == NavMeshPathStatus.PathInvalid)
        {
            token = GetRandomPointAccessibleByAgent(agent, currentTarget.transform, .1f);
            if (token.success)
            {
                return token;
            }

            return token;
        }
        else
        {
            return token;
        }
 
    }

    private PathReportToken GetRandomPointAccessibleByAgent(NavMeshAgent agent, Transform currentTarget, float range)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        PathReportToken token = new PathReportToken();
        Vector3 result = new Vector3();

        for (int i = 0; i < 15; i++)
        {
            Vector3 randomPoint = currentTarget.position + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, .5f, NavMesh.AllAreas))
            {
                result = hit.position;
            }
            else continue;

            //Debug.Log($"{currentTarget.gameObject.name} {result}");

            if (agent.CalculatePath(result, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                token.result = result;
                token.success = true;
                token.navMeshPath = navMeshPath;
                return token;
            }
        }
        //nyerah
        token.result = currentTarget.position;
        token.success = false;
        return token;
    }

    public Vector3 SampleClosestPosition(Vector3 currentTarget, float maxRange = 10)
    {
        NavMeshHit hit;
        bool validArea = false;
        if (NavMesh.SamplePosition(currentTarget, out hit, maxRange, NavMesh.AllAreas))
        {
            Vector3 result = hit.position;
            validArea = true;
        }

        if (NavMesh.FindClosestEdge(currentTarget, out hit, NavMesh.AllAreas))
        {

        }

        return hit.position;
    }

    public Vector3 CheckTargetClosestPosition(Vector3 currentTarget, float maxRange = 10)
    {
        NavMeshHit hit;
        Vector3 result = Vector3.zero;


        if (NavMesh.FindClosestEdge(currentTarget, out hit, NavMesh.AllAreas))
        {
            if (hit.distance < maxRange)
                return hit.position;
        }

        return currentTarget;
    }

    #endregion


    public RaycastHit GetHit(Vector3 pos, Vector3 dir, float range = 1000f, int layerMask = -99)
    {
        RaycastHit hit;
        int lm = Hypatios.Player.Weapon.defaultLayerMask;

        if (layerMask != -99)
        {
            lm = layerMask;
        }

        if (Physics.Raycast(pos, dir, out hit, range, lm, QueryTriggerInteraction.Ignore))
        {
        }
        else
        {
            hit.point = pos + (dir * range);
        }

        return hit;

    }

    [FoldoutGroup("Debug")] [ShowInInspector] [ReadOnly] private List<EnemyScript> tempList_NearestEnemy = new List<EnemyScript>();


    [FoldoutGroup("Debug")] [Button("testOrderDistance")]
    public void ListOrderTest(Transform enemyOrigin)
    {
        tempList_NearestEnemy.Clear();
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x => Vector3.Distance(enemyOrigin.transform.position, x.transform.position)).ToList();
    }

    /// <summary>
    /// Find any enemies/player to attack.
    /// </summary>
    /// <param name="alliance">Finder's alliance.</param>
    /// <param name="myPos">Finder's current world position.</param>
    /// <param name="chanceSelectAlly">Recommended value 0.1-1</param>
    public Entity FindEnemyEntity(Alliance alliance, Vector3 myPos = new Vector3(), float chanceSelectAlly = 0.3f, float maxDistance = 1000f)
    {
        tempList_NearestEnemy.Clear();
        tempList_NearestEnemy.RemoveAll(x => x.gameObject.activeInHierarchy == false);
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x => Vector3.Distance(myPos, x.transform.position)).ToList();
        tempList_NearestEnemy.RemoveAll(x => Vector3.Distance(myPos, x.transform.position) > maxDistance);

        return FindEnemyEntity(alliance, chanceSelectAlly);
    }

    [FoldoutGroup("Debug")] [ShowInInspector] [ReadOnly] private PathReportToken[] stupidTokens = new PathReportToken[5];
    [FoldoutGroup("Debug")] [ShowInInspector] [ReadOnly] private PathReportToken currentTOKEN;
    /// <summary>
    /// Find any enemies/player to attack. Using entityscript, specifically for navmeshagent.
    /// </summary>
    /// <param name="alliance">Finder's alliance.</param>
    /// <param name="entityScript">Finder's script.</param>
    /// <param name="chanceSelectAlly">Recommended value 0.1-1</param>
    private Entity FindEnemyEntity(Alliance alliance, Entity entityScript, float chanceSelectAlly = 0.3f, float maxDistance = 1000f, bool distanceNavmesh = false)
    {
        tempList_NearestEnemy.Clear();
        tempList_NearestEnemy.RemoveAll(x => x.gameObject.activeInHierarchy == false);
        EnemyScript mySelf = entityScript as EnemyScript;
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        if (tempList_NearestEnemy.Find(x => x == mySelf) != null) tempList_NearestEnemy.Remove(mySelf);

        if (distanceNavmesh == false)
        {
            tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x => Vector3.Distance(entityScript.transform.position, x.transform.position)).ToList();
            tempList_NearestEnemy.RemoveAll(x => Vector3.Distance(entityScript.transform.position, x.transform.position) > maxDistance);
        }
        else
        {
            tempList_NearestEnemy.RemoveAll(x => x.Stats.MainAlliance == alliance); //optimization method
            tempList_NearestEnemy.RemoveAll(x => x.gameObject.activeInHierarchy == false);
            tempList_NearestEnemy.RemoveAll(x => Vector3.Distance(entityScript.transform.position, x.transform.position) > maxDistance);
            stupidTokens = new PathReportToken[tempList_NearestEnemy.Count];
            int i = 0;

            tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x =>
            {
                var newtoken = GetDistanceByNavMesh(x.transform, entityScript);
                newtoken.objectName = $"{x.gameObject.name}";
                if (newtoken.navMeshPath != null) newtoken.distance = newtoken.navMeshPath.GetPathLength();
                if (newtoken.success == false) newtoken.distance = 999f;
                stupidTokens[i] = newtoken;
                float dist = newtoken.distance;
                i++;
                Debug.Log($"{x.gameObject.name} DIST: {dist}");
                return dist;
            }
            ).ToList();
            //tempList_NearestEnemy.Reverse();
            if (DEBUG_TestCheckUnit)
            {
                currentTOKEN = stupidTokens[0];
                debug_pathTarget.transform.position = stupidTokens[0].result;
            }
        }

        return FindEnemyEntity(alliance, chanceSelectAlly);
    }


    /// <summary>
    /// Find any enemies/player to attack. With UnitType filter.
    /// </summary>
    /// <param name="alliance">Finder's alliance.</param>
    /// <param name="unitType">Mechanical, boss, biological, etc</param>
    /// <param name="myPos">Finder's current world position.</param>
    /// <param name="chanceSelectAlly">Recommended value 0.1-1</param>
    /// <returns></returns>
    public Entity FindEnemyEntity(Alliance alliance, UnitType unitType, Vector3 myPos = new Vector3(), float chanceSelectAlly = 0.3f, float maxDistance = 1000f)
    {
        tempList_NearestEnemy.Clear();
        tempList_NearestEnemy.RemoveAll(x => x.gameObject.activeInHierarchy == false);
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x => Vector3.Distance(myPos, x.transform.position)).ToList();
        tempList_NearestEnemy.RemoveAll(x => Vector3.Distance(myPos, x.transform.position) > maxDistance);
        tempList_NearestEnemy.RemoveAll(x => x.Stats.UnitType != unitType);

        return FindEnemyEntity(alliance, chanceSelectAlly);
    }

    /// <summary>
    /// Checking enemies by screen's distance.
    /// </summary>
    /// <param name="alliance">Finder's alliance.</param>
    /// <param name="camTransform">Camera's transform.</param>
    /// <param name="chanceSelectAlly">Recommended value 0.1-1</param>
    /// <returns></returns>
    public Entity FindEnemyEntityFromScreen(Alliance alliance, Camera cam, float chanceSelectAlly = 0.3f)
    {
        tempList_NearestEnemy.Clear();
        tempList_NearestEnemy.RemoveAll(x => x.gameObject.activeInHierarchy == false);
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x =>
        {
            Vector3 screenPos = cam.WorldToScreenPoint(x.transform.position);
            float dist = Vector3.Distance(new Vector3(Screen.width/2f, Screen.height/2f, screenPos.z), screenPos);
            return Mathf.RoundToInt(dist*10)/10;
        })
        .ThenBy(l => 
        {
            return Vector3.Distance(cam.transform.position, l.transform.position);
        })
        .ToList();

        return FindEnemyEntity(alliance, chanceSelectAlly);
    }


    /// <summary>
    /// Get any enemies on the same faction.
    /// </summary>
    /// <param name="alliance"></param>
    /// <param name="chanceSelectAlly"></param>
    /// <returns></returns>
    private Entity GetRandomEnemyEntity(Alliance alliance)
    {
        var enemies = AllEnemies.FindAll(x => x.Stats.MainAlliance == alliance && x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile);
        var enemy = enemies[Random.Range(0, enemies.Count - 1)];

        return enemy;
    }


    private Entity FindEnemyEntity(Alliance alliance, float chanceSelectAlly = 0.3f)
    {
        //base enemy entity searcher
        tempList_NearestEnemy.RemoveAll(x => x.gameObject.activeInHierarchy == false);

        if (alliance != Alliance.Player)
        {
            float chance = Random.Range(-0f, 1f);

            if (chance < chanceSelectAlly && CountMyEnemies(alliance) > 0)
            {
                var enemy = tempList_NearestEnemy.Find(x => x.Stats.MainAlliance != alliance && x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile && x.Stats.IsDead == false);

                return enemy;
            }
            else
            {
                return Hypatios.Player;
            }
        }

        else if (alliance == Alliance.Rogue)
        {
            var enemy = tempList_NearestEnemy.Find(x => x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile);
            return enemy;
        }
        else
        {
            var enemy = tempList_NearestEnemy.Find(x => x.Stats.MainAlliance != alliance && x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile);
            return enemy;

        }
    }

    public int CheckMyIndex(EnemyScript enemyScript)
    {
        return AllEnemies.IndexOf(enemyScript);
    }

    /// <summary>
    /// Check if the targeted transform is a part of an enemy.
    /// </summary>
    /// <param name="target">Gameobject that has EnemyScript.cs or leg of an enemy + not the same alliance, will return true.</param>
    /// <param name="myAlliance"></param>
    /// <returns></returns>
    public bool CheckTransformIsAnEnemy(Transform target, Alliance myAlliance)
    {
        var enemy = target.GetComponent<EnemyScript>();
        if (enemy == null) enemy = target.GetComponentInParent<EnemyScript>();

        if (enemy != null)
        {
            if (enemy.Stats.MainAlliance != myAlliance)
                return true;
        }

        return false;
    }


}
