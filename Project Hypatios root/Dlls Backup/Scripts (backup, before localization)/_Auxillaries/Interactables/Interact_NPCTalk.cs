using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for generic NPCs
public class Interact_NPCTalk : MonoBehaviour
{

    public Interact_MultiDialoguesTrigger dialog_firstTimeMet;
    public Interact_MultiDialoguesTrigger dialog_everMet;
    public string ID = "Level2.DrPlague.Encounter";

    public void Talk()
    {
        string key = $"NPC-Talk.{ID}";

        if (Hypatios.Game.Check_ParadoxEvent(key) == false)
        {
            dialog_firstTimeMet.TriggerMessage();
            Hypatios.Game.TryAdd_ParadoxEvent(key);
        }
        else
        {
            dialog_everMet.TriggerMessage();
        }
    }

}
