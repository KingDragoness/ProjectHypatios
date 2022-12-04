using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{

    public OnTriggerEnterEvent TriggerScript;
    public float Value = 0;
    public StatusEffectCategory statusEffect;

    private GenericStatus genericStatus;

    public void CreateAOE()
    {
        var charScript = TriggerScript.objectToCompare.GetComponent<CharacterScript>();

        if (genericStatus != null)
        {
            Destroy(genericStatus.gameObject);
        }

        genericStatus = charScript.CreatePersistentStatusEffect(statusEffect, Value, gameObject.name);
    }

    public void ExitAOE()
    {
        if (genericStatus != null)
        {
            Destroy(genericStatus.gameObject);
        }
    }

}
