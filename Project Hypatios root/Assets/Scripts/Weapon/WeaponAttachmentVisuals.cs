using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WeaponAttachmentVisuals : MonoBehaviour
{

    public string ID = "StandardReceiver";
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

}
