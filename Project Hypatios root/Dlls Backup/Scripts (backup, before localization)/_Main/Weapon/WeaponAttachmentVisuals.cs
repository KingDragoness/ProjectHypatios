using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


//Modular script! Don't tie to GunScript, also used in display weapon
public class WeaponAttachmentVisuals : MonoBehaviour
{

    public string ID = "StandardReceiver";
    public UnityEvent OnRequirementMet;
    public UnityEvent OnNotMet;
    public GameObject visual;

    public void RefreshVisuals(string unlockID)
    {
        if (unlockID == ID)
        {
            visual.gameObject.SetActive(true);
        }
        else
        {
            visual.gameObject.SetActive(false);
        }
    }

    public void TriggerRequirements(bool isMet)
    {
        if (isMet == true)
        {
            OnRequirementMet?.Invoke();

        }
        else
        {
            OnNotMet?.Invoke();

        }
    }

}
