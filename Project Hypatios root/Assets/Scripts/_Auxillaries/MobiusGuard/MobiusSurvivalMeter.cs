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
    [FoldoutGroup("Hitpoint")] public float HP_surviveMultiplier = 0.4f;
    [FoldoutGroup("Hitpoint")] public float HP_lowerConfidentEscape = -10f;
    [FoldoutGroup("Squad")] public float minSquadDist = 2f;
    [FoldoutGroup("Squad")] public float softSquadDist = 10f;
    [FoldoutGroup("Squad")] public float maxSquadDist = 30f;
    [FoldoutGroup("Squad")] public float perSquadPower = 2f;
    public MobiusGuardEnemy mobiusGuardEnemy;

    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (mobiusGuardEnemy.isAIEnabled == false) return;
        float survivalIndex = 0;

        {
            float HP_index = 0;

            if (mobiusGuardEnemy.Stats.CurrentHitpoint < thresHP_Escape)
            {
                HP_index = 100f - 100f * (Mathf.Clamp(mobiusGuardEnemy.Stats.CurrentHitpoint,0, 99999f) / thresHP_Escape);
                HP_index = -HP_index;
                HP_index -= HP_lowerConfidentEscape;
            }
            if (mobiusGuardEnemy.Stats.CurrentHitpoint > thresHP_Confident)
            {
                HP_index = 100f * (mobiusGuardEnemy.Stats.CurrentHitpoint / mobiusGuardEnemy.Stats.MaxHitpoint.Value);
            }

            HP_index *= HP_surviveMultiplier;
            survivalIndex += HP_index;
        }

        //assess how many enemies near me
        {
            float squad_Index = 0;

            foreach(var guard in MobiusGuardEnemy.AllActiveGuards)
            {
                if (guard == null) continue;
                if (guard == mobiusGuardEnemy) continue;
                float dist = Vector3.Distance(guard.transform.position, mobiusGuardEnemy.transform.position);
                float power = 0;

                if (dist < minSquadDist)
                {
                    power = perSquadPower;
                }
                else if (dist < softSquadDist && dist > minSquadDist)
                {
                    power = perSquadPower;
                }
                
                if (dist > softSquadDist && dist < maxSquadDist)
                {
                    float max = maxSquadDist - softSquadDist;
                    float x = (dist - softSquadDist) / max;
                    x = 1 - x;
                    float z = Mathf.Lerp(0, max, x);
                    power += z * perSquadPower;
                }

                squad_Index += power;
            }

            survivalIndex += squad_Index;
        }

        survivalIndex += mobiusGuardEnemy.confidenceLevel * 0.1f;
        survivalIndex = Mathf.Clamp(survivalIndex, -100f, 100f);
        mobiusGuardEnemy.survivalEngageLevel = survivalIndex; 

    }

}
