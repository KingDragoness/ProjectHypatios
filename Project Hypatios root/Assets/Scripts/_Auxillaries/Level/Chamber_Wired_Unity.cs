using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber_Wired_Unity : MonoBehaviour
{

    public string prompt_LoseSoulReload;

    public void LoseOneSoul()
    {
        if (Hypatios.Game.SoulPoint > 0f) Hypatios.Game.SoulPoint -= 1;
        DeadDialogue.PromptNotifyMessage_Mod(prompt_LoseSoulReload, 4f);
    }

}
