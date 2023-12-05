using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AddStatusEffect : MonoBehaviour
{

    public BaseStatusEffectObject statusEffect;
    public float timeEffect = 9999f;
    public float promptTime = 4f;
    public bool usePrompt_StatusEffect = false;
    public string prompt_StatusEffect = "Cursed by Eclipser sword.";

    public void AddStatus()
    {
        statusEffect.AddStatusEffectPlayer(timeEffect);

        if (usePrompt_StatusEffect)
        {
            DeadDialogue.PromptNotifyMessage_Mod(prompt_StatusEffect, promptTime);

        }
    }

    public void RemoveStatus()
    {
        Hypatios.Player.RemoveStatusEffectGroup(statusEffect);
    }

}
