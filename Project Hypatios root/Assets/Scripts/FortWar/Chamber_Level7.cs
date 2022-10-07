using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Chamber_Level7 : MonoBehaviour
{

    public enum Stage
    {
        NotStart,
        Ongoing,
        Finished
    }

    [FoldoutGroup("Objects")] public List<FW_ControlPoint> controlPoint;
    [FoldoutGroup("Objects")] public Transform containerUnits;
    [FoldoutGroup("Objects")] public Enemy_FW_Bot defenderGuardstar;
    [FoldoutGroup("Objects")] public Enemy_FW_Bot invaderGuardstar;
    [FoldoutGroup("Objects")] [ReadOnly] public List<FW_ControlPoint> capturedCP;
    [FoldoutGroup("Objects")] [ReadOnly] public List<FW_StrategicPoint> strategicPoints;
    [FoldoutGroup("Debug")] public GameObject debug_Canvas;
    [FoldoutGroup("Debug")] public Text debug_Text1;
    [FoldoutGroup("Debug")] public Image[] debug_ControlPoints;

    public Stage currentStage;
    [ReadOnly] private List<FW_Targetable> allUnits = new List<FW_Targetable>();

    public static Chamber_Level7 instance;
    public const int MAXIMUM_PLAYER_TEAM = 20;
    public delegate void OnModifiedFollower();
    public event OnModifiedFollower onModifiedFollower;

    public List<FW_Targetable> AllUnits { get => allUnits;  }
    public int InvaderUnitCount { get => allUnits.Where(z => z.Alliance == FW_Alliance.INVADER && z.UnitType == FW_Targetable.Type.Bot).ToList().Count; }
    public int DefenderUnitCount { get => allUnits.Where(z => z.Alliance == FW_Alliance.DEFENDER && z.UnitType == FW_Targetable.Type.Bot).ToList().Count; }

    private FW_ControlPoint finalCP;
    private List<Enemy_FW_BotTest> botFollowers = new List<Enemy_FW_BotTest>();
    public int FollowerCount { get => botFollowers.Count; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        finalCP = controlPoint.Find(x => x.CPNumber == 0);
    }

    private void Update()
    {
        allUnits.RemoveAll(x => x == null);

        if (debug_Canvas.activeSelf)
            UpdateDebugScreen();

        if (finalCP.isCaptured)
        {
            if (currentStage == Stage.Ongoing)
                ClearedChamber();
        }
    }

    public bool AddFollower(Enemy_FW_BotTest bot)
    {
        if (botFollowers.Count <= 4)
        {
            botFollowers.Add(bot);
            onModifiedFollower?.Invoke();
            return true;
        }

        return false;
    }

    public void RemoveFollower(Enemy_FW_BotTest bot)
    {
        botFollowers.Remove(bot);
        onModifiedFollower?.Invoke();

    }

    private void UpdateStat()
    {

    }

    private void UpdateDebugScreen()
    {
        var allInvaders = allUnits.Where(z => z.Alliance == FW_Alliance.INVADER).ToList();
        var allDefenders = allUnits.Where(z => z.Alliance == FW_Alliance.DEFENDER).ToList();

        string s1 = "";
        s1 += $"(D: {allDefenders.Count}/{MAXIMUM_PLAYER_TEAM}) (I: {allInvaders.Count}/{MAXIMUM_PLAYER_TEAM})";

        debug_Text1.text = s1;
    }

    [FoldoutGroup("Chamber")] public GameObject sign_LevelStateCleared;
    [FoldoutGroup("Chamber")] public GameObject sign_LevelStateUnclear;
    [FoldoutGroup("Chamber")] public Animator doorAnimator;
    [FoldoutGroup("Chamber")] public AudioSource chamberAudioAnnouncement;
    [FoldoutGroup("Chamber")] public UnityEvent OnChamberCompleted;

    private void ClearedChamber()
    {
        doorAnimator.SetBool("IsOpened", true);
        sign_LevelStateCleared.gameObject.SetActive(true);
        sign_LevelStateUnclear.gameObject.SetActive(false);

        if (currentStage != Stage.Finished)
        {
            if (chamberAudioAnnouncement != null)
            {
                chamberAudioAnnouncement.Play();
                OnChamberCompleted?.Invoke();
                DialogueSubtitleUI.instance.QueueDialogue("Attention to all facility users: Chamber completed.", "ANNOUNCER", 14f);
            }
        }

        currentStage = Stage.Finished;
    }

    #region Bots
    public void RegisterUnit(FW_Targetable bot)
    {
        allUnits.Add(bot);
        bot.transform.SetParent(containerUnits);
    }
    public void DeregisterUnit(FW_Targetable bot)
    {
        allUnits.Remove(bot);
    }
    public List<FW_Targetable> RetrieveAllUnitsOfType(FW_Alliance alliance)
    {
        var list1 = new List<FW_Targetable>();
        list1.AddRange(allUnits);
        list1.RemoveAll(x => x.Alliance != alliance);
        return list1;
    }

    #endregion

    #region Cheat Commands

    public void Comm_ToggleDebugScreen()
    {
        debug_Canvas.SetActive(!debug_Canvas.activeSelf);
    }

    public void Comm_SetStage(int stage)
    {
        currentStage = (Stage)stage;
    }

    #endregion

}
