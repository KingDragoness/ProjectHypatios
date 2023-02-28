using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Chamber_Wired_Heaven : MonoBehaviour
{

    public AnimationPlayerScript gateCloseAnim;
    public BossOrphanimEnemy bossEnemy;
    public GameObject cutsceneObject;
    public UnityEvent OnBattleStart;

    public void InitiateBossFight()
    {
        bossEnemy.ChangeStage(BossOrphanimEnemy.Stage.Battle);
    }

    public void InitiateBattle()
    {
        gateCloseAnim.PlayAnimation();
        cutsceneObject.gameObject.SetActive(false);
        OnBattleStart?.Invoke();
    }

}
