using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlagTrigger : MonoBehaviour
{

    public GlobalFlagSO flag;
    public int lastHowManyRun = 1;

    [ContextMenu("Trigger Flag")]
    public void TriggerFlag()
    {
        Hypatios.Game.TriggerFlag(flag.GetID(), lastHowManyRun);
    }


    [ContextMenu("Remove Flag")]
    public void RemoveFlag()
    {
        var flagData = Hypatios.Game.Game_GlobalFlags.Find(x => x.ID == flag.GetID());
        Hypatios.Game.Game_GlobalFlags.Remove(flagData);
    }

}
