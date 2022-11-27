using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Utility;

public class Chamber_Boss : MonoBehaviour
{

    public GameObject catwalkEpic;
    public Vector3 catwalkScriptSpeed = new Vector3(0, 1, 0);
    public AutoMoveAndRotate catwalkScript;
    public AudioSource music_FirstStage;
    public AudioSource music_SecondStage;
    public HyperchadEnemy hyperchadEnemy;
    [Space]
    [Header("Final Form")]
    public Vector3 downwardPlatformSpeed = new Vector3(0, -10, 0);
    public float delayPerPlatform = 0.5f;
    public UnityEvent OnFinalFormTrigger;
    public UnityEvent OnPrepareFinalFormTrigger;
    public List<GameObject> PlatformToBeDismantled = new List<GameObject>();
    [Space]
    [Header("Ending Sequence")]
    public UnityEvent OnBossKilled;
    public GameObject corpseNPC;

    private bool hasEnteredBattle = false;
    private bool trigger_PrepareFinalForm = false;
    private bool trigger_FinalForm = false;
    private bool trigger_BossDied = false;

    private void Update()
    {
        if (hasEnteredBattle)
        {
            bool isHealthHalf = (hyperchadEnemy.Stats.CurrentHitpoint / hyperchadEnemy.Stats.MaxHitpoint.Value) < 0.5f ? true : false;
            bool isHealthQuarter = (hyperchadEnemy.Stats.CurrentHitpoint / hyperchadEnemy.Stats.MaxHitpoint.Value) < 0.3f ? true : false;
            bool isDead = (hyperchadEnemy.Stats.CurrentHitpoint) <= 0f ? true : false;

            if (catwalkEpic.transform.position.y > -100)
            {
                catwalkScript.moveUnitsPerSecond.value = catwalkScriptSpeed;
                catwalkScript.enabled = true;
            }
            else
            {
                catwalkScript.enabled = false;
            }

            if (isHealthQuarter)
            {
                if (trigger_PrepareFinalForm == false)
                {
                    TriggerPrepareFinalForm();
                }
            }

            if (hyperchadEnemy.currentStance == HyperchadEnemy.MoveStances.FinalForm)
            {
                if (trigger_FinalForm == false)
                {
                    TriggerFinalForm();
                }

                RunFinalForm();
            }

            if (isDead)
            {
                if (trigger_BossDied == false)
                {
                    BossDied();
                }
            }

        }
    }

    private void RunFinalForm()
    {
        foreach (var platform in PlatformToBeDismantled)
        {
            if (platform.transform.position.y < -120)
            {
                platform.SetActive(false);
            }
        }
    }

    [ContextMenu("TriggerPrepareFinalForm")]
    public void TriggerPrepareFinalForm()
    {
        music_FirstStage.Stop();
        hyperchadEnemy.currentStance = HyperchadEnemy.MoveStances.PrepareFinal;
        hyperchadEnemy.PrepareFinalForm();
        OnPrepareFinalFormTrigger?.Invoke();
        trigger_PrepareFinalForm = true;
    }

    [ContextMenu("TriggerFinalForm")]
    public void TriggerFinalForm()
    {
        music_SecondStage.gameObject.SetActive(true);
        StartCoroutine(DismantlePlatform());
        OnFinalFormTrigger?.Invoke();
        trigger_FinalForm = true;
    }

    public void BossDied()
    {
        music_FirstStage.Stop();
        music_SecondStage.Stop();
        hyperchadEnemy.gameObject.SetActive(false);
        corpseNPC.gameObject.SetActive(true);
        corpseNPC.transform.position = hyperchadEnemy.transform.position;

        OnBossKilled?.Invoke();
    }

    IEnumerator DismantlePlatform()
    {
        yield return new WaitForSeconds(1);
        List<AutoMoveAndRotate> allAutoMoves = new List<AutoMoveAndRotate>();

        foreach (var platform in PlatformToBeDismantled)
        {
            var amr1 = platform.AddComponent<AutoMoveAndRotate>();
            amr1.moveUnitsPerSecond = new AutoMoveAndRotate.Vector3andSpace();
            amr1.moveUnitsPerSecond.value = downwardPlatformSpeed;
            amr1.rotateDegreesPerSecond = new AutoMoveAndRotate.Vector3andSpace();
            allAutoMoves.Add(amr1);

            yield return new WaitForSeconds(delayPerPlatform);
        }


        foreach (var move in allAutoMoves)
        {
            //move.moveUnitsPerSecond.value = move.moveUnitsPerSecond.value * 10;
        }

    }

    public void TriggerEnterBattle()
    {
        hasEnteredBattle = true;
    }

}
