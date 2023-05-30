using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class MobiusSurvivalMeter : MonoBehaviour
{

    [FoldoutGroup("Hitpoint")] public float thresHP_Escape = 100f;
    [FoldoutGroup("Hitpoint")] public float thresHP_Confident = 200f;
    public MobiusGuardEnemy mobiusGuardEnemy;

    private void Update()
    {
        if (mobiusGuardEnemy.isAIEnabled == false) return;
        float survivalIndex = 0;

        {
            float HP_index = 0;

            if (mobiusGuardEnemy.Stats.CurrentHitpoint < thresHP_Escape)
            {
                HP_index = -100f * (mobiusGuardEnemy.Stats.CurrentHitpoint / thresHP_Escape);
            }
            if (mobiusGuardEnemy.Stats.CurrentHitpoint > thresHP_Confident)
            {
                HP_index = 100f * (mobiusGuardEnemy.Stats.CurrentHitpoint / mobiusGuardEnemy.Stats.MaxHitpoint.Value);
            }

            survivalIndex += HP_index;
        }

        survivalIndex = Mathf.Clamp(survivalIndex, -100f, 100f);
        mobiusGuardEnemy.survivalEngageLevel = survivalIndex; 

    }

}
