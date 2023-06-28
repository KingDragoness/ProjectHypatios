using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Animancer;

public class Interact_NPC : MonoBehaviour
{

    public ClipTransition idleAnimation;
    [FoldoutGroup("References")] public Animator animator;
    [FoldoutGroup("References")] public AnimancerPlayer AnimatorPlayer;
    public List<Interact_MultiDialoguesTrigger> dialoguePrefabs;

    public static int[] everTalkedIndexes = new int[5];
    public static int lastIndex = 0;

    private void Start()
    {
        AnimatorPlayer.PlayAnimation(idleAnimation, 1f);
    }

    public void Speak()
    {
        int _index = Random.Range(0, dialoguePrefabs.Count - 1);
        int count = 0;

        while (everTalkedIndexes.Contains(_index))
        {
            _index = Random.Range(0, dialoguePrefabs.Count - 1);
            count++;
            if (count > 100) break;
        }

        everTalkedIndexes[lastIndex] = _index;
        Interact_MultiDialoguesTrigger dialogue = dialoguePrefabs[lastIndex];
        var objectPrefab1 = Instantiate(dialogue);
        objectPrefab1.TriggerMessage();
        Destroy(objectPrefab1, 1f);

        lastIndex++;
        if (lastIndex > everTalkedIndexes.Length - 1) lastIndex = 0;
    }

}
