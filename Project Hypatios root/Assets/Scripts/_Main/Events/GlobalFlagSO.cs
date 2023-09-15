using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Antagonize_Faction_HEV", menuName = "Hypatios/Flag Event", order = 1)]

public class GlobalFlagSO : ScriptableObject
{

    [SerializeField] private int _priority = 10000;
    [SerializeField] private string _displayName = "I have antagonized the HEV faction.";
    [SerializeField] [TextArea(3,5)] private string _description = "";
    [SerializeField] private GameObject prefabToSpawn;

    public string Description { get => _description;}
    public GameObject PrefabToSpawn { get => prefabToSpawn;  }
    public string DisplayName { get => _displayName;  }

    public string GetID()
    {
        return name;
    }

    [FoldoutGroup("DEBUG")] [Button("DEBUG_TriggerFlag")]
    public void TriggerFlag(int _run = 1)
    {
        Hypatios.Game.TriggerFlag(name, _run);
    }
}
