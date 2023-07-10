using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Animancer;

public class Interact_NPC : MonoBehaviour
{

    public enum ConverseType
    {
        Random,
        Incremental
    }

    public ClipTransition idleAnimation;
    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public AnimancerPlayer AnimatorPlayer;
    public List<Interact_MultiDialoguesTrigger> dialoguePrefabs;
    public ConverseType type = ConverseType.Random;
    [ShowIf("type", ConverseType.Incremental)] public string Paradox_Key = "HeerEtresSoldier";
    [ShowIf("type", ConverseType.Incremental)] [InfoBox("If run out, it'll fallback to random talks from dialoguePrefabs.")] public List<Interact_MultiDialoguesTrigger> incrementDialogue;


    private void Start()
    {
        AnimatorPlayer.PlayAnimation(idleAnimation, 1f);
    }

    public void Speak()
    {
        if (type == ConverseType.Random)
        {
            Speak_Random();
        }
        else
        {
            Speak_Incremental();
        }

    }


    private void Speak_Random()
    {
        int _index = Random.Range(0, dialoguePrefabs.Count - 1);
        int count = 0;

        Interact_MultiDialoguesTrigger dialogue = dialoguePrefabs[_index];
        var objectPrefab1 = Instantiate(dialogue);
        objectPrefab1.TriggerMessage();
        Destroy(objectPrefab1, 1f);
        Debug.Log(objectPrefab1.gameObject.name);
    }

    private void Speak_Incremental()
    {
        var value = Hypatios.Game.GetParadoxEntityValue($"NPC.{Paradox_Key}");
        int i = 0;

        if (int.TryParse(value, out i)) {   }

        if (i >= incrementDialogue.Count)
        {
            Speak_Random();
            return;
        }


        Interact_MultiDialoguesTrigger dialogue = incrementDialogue[i];
        var objectPrefab1 = Instantiate(dialogue);
        objectPrefab1.TriggerMessage();
        Destroy(objectPrefab1, 1f);
        Debug.Log(objectPrefab1.gameObject.name);
        i++;
        Hypatios.Game.SetParadoxEntity($"NPC.{Paradox_Key}", i.ToString());
    }

}
