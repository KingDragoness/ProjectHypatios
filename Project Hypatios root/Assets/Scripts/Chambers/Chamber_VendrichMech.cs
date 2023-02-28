using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static MechHeavenblazerEnemy;

public class Chamber_VendrichMech : MonoBehaviour
{

    public MechHeavenblazerEnemy mechEnemy;
    public Chamber_Vendrich_HealingTower healingTower;
    public RandomSpawnArea spawnTowerArea;
    [FoldoutGroup("Stage Object")] public GameObject Stage0_intro;
    [FoldoutGroup("Stage Object")] public GameObject Stage1_fightOn;
    [FoldoutGroup("Stage Object")] public GameObject Stage2_redDust;
    [FoldoutGroup("Stage Object")] public GameObject Stage3_ascension;
    [FoldoutGroup("Stage Object")] public GameObject Stage4_lastMessage;
    [FoldoutGroup("Stage Object")] public GameObject DeathStage;

    [FoldoutGroup("Parameters")] public float hTower_distMinimum = 40;
    [FoldoutGroup("Parameters")] public int hTower_limitSpawnTries = 50;
    [FoldoutGroup("Parameters")] public float hTower_TimerSpawn = 35f;
    [FoldoutGroup("Parameters")] [Range(0f,1f)] public float hTower_SpawnChance = 0.3f;
    [FoldoutGroup("Parameters")] [Range(0f, 1f)] public float hTower_SpawnChance_Ascen = 0.2f;
    [FoldoutGroup("Parameters")] public float hTower_limitMechHP = 80000f;
    [FoldoutGroup("Parameters")] public float triggerHP_RedDust = 60000f;
    [FoldoutGroup("Parameters")] public float triggerHP_Ascension = 40000f;
    [FoldoutGroup("Parameters")] public float triggerHP_LastMessage = 20000f;
    public bool DEBUG_DrawGizmos = false;
    public bool DEBUG_DrawGUI = false;


    [FoldoutGroup("Layout")] public float offsetX = 20f;
    [FoldoutGroup("Layout")] public float perLineYSize = 20f;
    [FoldoutGroup("Layout")] public Vector3 EnemyWindowSize = new Vector3(300, 100);
    [FoldoutGroup("Layout")] public Vector3 EnemyWindowOffset = new Vector3(30, 20);
    public GUISkin skin1; //right handed header

    private float _spawnHealingTowerTimer = 0f;
    private bool _isRedDustTriggered = false;
    private bool _isAscensionTriggered = false;
    private bool _isLastMessageTriggered = false;
    private bool _isDeathTriggered = false;

    private void Start()
    {
        _spawnHealingTowerTimer = hTower_TimerSpawn;
        healingTower.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        CheckConditions();
        RefreshStage();

        if (_spawnHealingTowerTimer > 0f)
        {
            _spawnHealingTowerTimer -= Time.deltaTime;
        }
        else
        {
            float chance = Random.Range(0f, 1f);
            bool allowSpawn = true;

            if (hTower_SpawnChance < chance) allowSpawn = false;
            if (mechEnemy.Stats.CurrentHitpoint >= hTower_limitMechHP) allowSpawn = false;
            if (mechEnemy.currentStage == Stage.Stage4_LastMessage) allowSpawn = false;
            if (mechEnemy.currentStage == Stage.Death) allowSpawn = false;

            if (allowSpawn)
            {
                if (mechEnemy.currentStage == Stage.Stage3_Ascend)
                {
                    if (hTower_SpawnChance_Ascen > chance)
                        SpawnHealingTower();
                }
                else
                    SpawnHealingTower();
            }
            
            _spawnHealingTowerTimer = hTower_TimerSpawn;
        }
    }

    private void CheckConditions()
    {
        if (_isRedDustTriggered == false)
        {
            if (mechEnemy.Stats.CurrentHitpoint < triggerHP_RedDust)
            {
                mechEnemy.ForceChangeStage(Stage.Stage2_Dust);
            }
        }

        if (_isAscensionTriggered == false)
        {
            if (mechEnemy.Stats.CurrentHitpoint < triggerHP_Ascension)
            {
                mechEnemy.ForceChangeStage(Stage.Stage3_Ascend);
            }
        }

        if (_isLastMessageTriggered == false)
        {
            if (mechEnemy.Stats.CurrentHitpoint < triggerHP_LastMessage)
            {
                mechEnemy.ForceChangeStage(Stage.Stage4_LastMessage);
            }
        }
    }

    private void RefreshStage()
    {
        if (mechEnemy.currentStage == Stage.Stage0_Intro)
        {
            if (Stage0_intro.activeSelf == false) Stage0_intro.gameObject.SetActive(true);
        }
        else
        {
            if (Stage0_intro.activeSelf == true) Stage0_intro.gameObject.SetActive(false);
        }

        if (mechEnemy.currentStage == Stage.Stage1_Fight)
        {
            if (Stage1_fightOn.activeSelf == false) Stage1_fightOn.gameObject.SetActive(true);
        }
        else
        {
            if (Stage1_fightOn.activeSelf == true) Stage1_fightOn.gameObject.SetActive(false);
        }

        if (mechEnemy.currentStage == Stage.Stage2_Dust)
        {
            _isRedDustTriggered = true;
            if (Stage2_redDust.activeSelf == false) Stage2_redDust.gameObject.SetActive(true);
        }
        else
        {
            if (Stage2_redDust.activeSelf == true) Stage2_redDust.gameObject.SetActive(false);
        }

        if (mechEnemy.currentStage == Stage.Stage3_Ascend)
        {
            _isAscensionTriggered = true;
            if (Stage3_ascension.activeSelf == false) Stage3_ascension.gameObject.SetActive(true);
        }
        else
        {
            if (Stage3_ascension.activeSelf == true) Stage3_ascension.gameObject.SetActive(false);
        }

        if (mechEnemy.currentStage == Stage.Stage4_LastMessage)
        {
            _isLastMessageTriggered = true;
            if (Stage4_lastMessage.activeSelf == false) Stage4_lastMessage.gameObject.SetActive(true);
        }
        else
        {
            if (Stage4_lastMessage.activeSelf == true) Stage4_lastMessage.gameObject.SetActive(false);
        }

        if (mechEnemy.currentStage == Stage.Death)
        {
            _isDeathTriggered = true;
            if (DeathStage.activeSelf == false) DeathStage.gameObject.SetActive(true);
        }
        else
        {
            if (DeathStage.activeSelf == true) DeathStage.gameObject.SetActive(false);
        }
    }


    #region Spawning

    [FoldoutGroup("Debug")] [Button("Spawn Healing Tower")]
    public void SpawnHealingTower()
    {
        var pos = spawnTowerArea.GetAnyPositionInsideBox();
        float dist = Vector3.Distance(pos, Hypatios.Player.transform.position);
        bool valid = false;

        int t = 0;

        while (valid == false && t < hTower_limitSpawnTries && t < 1000)
        {
            t++;
            if (dist < hTower_distMinimum)
                valid = true;
            else
                pos = spawnTowerArea.GetAnyPositionInsideBox();
        }

        var newTower = Instantiate(healingTower, pos, Quaternion.identity);
        newTower.gameObject.SetActive(true);
        Vector3 rot = newTower.transform.eulerAngles;
        rot.y = Random.Range(0f, 360f);
        newTower.Init();
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (DEBUG_DrawGizmos == false)
            return;

        if (mechEnemy.CurrentAI is HB_Stance_PatrolGroundWalk)
        {
            Color c1 = Color.red;
            c1.a = 0.2f;
            Gizmos.color = c1;
            Gizmos.DrawSphere(mechEnemy.patrolground_WalkTarget, 1f);
            Gizmos.DrawSphere(mechEnemy.transform.position, 1f);
            c1.a = 0.9f;
            Gizmos.color = c1;
            Gizmos.DrawLine(mechEnemy.transform.position, mechEnemy.patrolground_WalkTarget);
        }
    }

    private void OnGUI()
    {
        if (DEBUG_DrawGUI == false)
            return;

        StageAvailableAbilities csd = mechEnemy.allStageSystems.Find(x => x.stage == mechEnemy.currentStage);
        string s1 = $"Stage: {mechEnemy.currentStage} | AI_Pack: {mechEnemy.CurrentAI.name}\n";
        s1 += $"[AI_Time: {Mathf.Round(mechEnemy.RefreshChangeStageTime*10)/10}s]\n";

        string s2 = $"\n[DECISIONS ({mechEnemy.GetTotalWeight(csd.decisions)})]\n";
        foreach(var decision in csd.decisions)
        {
            s2 += $"{decision.package.name} : {decision.GetNetWeight(mechEnemy)}\n";
        }

        s1 += s2;
        int countLine = s1.CountLines();

        GUI.Box(new Rect(EnemyWindowOffset.x, EnemyWindowOffset.y, EnemyWindowSize.x, perLineYSize * countLine), s1
                , skin1.box);
    }

}
