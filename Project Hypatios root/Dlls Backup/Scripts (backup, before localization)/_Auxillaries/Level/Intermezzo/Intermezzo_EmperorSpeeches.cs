using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class Intermezzo_EmperorSpeeches : MonoBehaviour
{

    public List<Interact_MultiDialoguesTrigger> allSpeeches = new List<Interact_MultiDialoguesTrigger>();
    public Interact_MultiDialoguesTrigger defaultSpeech;

    [FoldoutGroup("Debug")] [Button("Scan Dialogues")]
    public void ScanDialogues()
    {
        allSpeeches = gameObject.GetComponentsInChildren<Interact_MultiDialoguesTrigger>().ToList();
    }

    public void Talk()
    {
        int i = ChamberLevelController.Instance.GetChamberCompletion();
        var currentSpeech = GetDialogue(i);
        currentSpeech.TriggerMessage();
    }

    public Interact_MultiDialoguesTrigger GetDialogue(int index)
    {
        if (allSpeeches.Count() <= index)
        {
            return defaultSpeech;
        }


        return allSpeeches[index];
    }
}
