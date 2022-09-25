using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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
    [FoldoutGroup("Debug")] public GameObject debug_Canvas;
    [FoldoutGroup("Debug")] public Text debug_Text1;
    [FoldoutGroup("Debug")] public Image[] debug_ControlPoints;

    public Stage currentStage;
    [ReadOnly] private List<FW_Targetable> allUnits = new List<FW_Targetable>();

    public static Chamber_Level7 instance;
    public const int MAXIMUM_PLAYER_TEAM = 20;

    public List<FW_Targetable> AllUnits { get => allUnits;  }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (debug_Canvas.activeSelf)
            UpdateDebugScreen();
    }

    private void UpdateDebugScreen()
    {
        var allInvaders = allUnits.Where(z => z.Alliance == FW_Alliance.INVADER).ToList();
        var allDefenders = allUnits.Where(z => z.Alliance == FW_Alliance.DEFENDER).ToList();

        string s1 = "";
        s1 += $"(D: {allDefenders.Count}/{MAXIMUM_PLAYER_TEAM}) (I: {allInvaders.Count}/{MAXIMUM_PLAYER_TEAM})";

        debug_Text1.text = s1;
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

    #endregion

}
