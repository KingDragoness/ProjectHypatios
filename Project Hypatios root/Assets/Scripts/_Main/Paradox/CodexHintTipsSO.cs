using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "How To Kill Yourself", menuName = "Hypatios/Codex Hint", order = 1)]

public class CodexHintTipsSO : ScriptableObject
{

    [SerializeField] private string ID = "DiseasesAilments";
    public string Title = "Diseasesand Ailments";

    [TextArea(3, 6)] public string Description = "At random times, Aldrich can suffer from Depression, Fatigue, Rage and Panic Attack ailments.";

    public string RawID { get => ID; }

    public string GetID()
    {
        return $"Codex.{ID}";
    }

    [FoldoutGroup("DEBUG")] [Button("Trigger Codex")]
    public void TriggerCodex()
    {
        Hypatios.Game.RuntimeTutorialHelp(this);
    }

}
