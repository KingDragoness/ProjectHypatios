using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chamber3_MarketplaceScript : MonoBehaviour
{

    public GameObject trigger_NotFoundYet;
    public GameObject trigger_Found;
    public GameObject trigger_Generic;
    public int collectedHerbs = 0;

    public void AddHerb()
    {
        collectedHerbs++;
        CheckTrigger();
    }

    [Button("Check Trigger")]
    private void CheckTrigger()
    {
        if (collectedHerbs >= 8)
        {
            trigger_NotFoundYet.gameObject.SetActive(false);
            trigger_Found.gameObject.SetActive(true);
            trigger_Generic.gameObject.SetActive(false);
        }
    }

    public void SetDialogueGeneric()
    {
        FPSMainScript.instance.SoulPoint += 50;
        trigger_NotFoundYet.gameObject.SetActive(false);
        trigger_Found.gameObject.SetActive(false);
        trigger_Generic.gameObject.SetActive(true);

    }

}
