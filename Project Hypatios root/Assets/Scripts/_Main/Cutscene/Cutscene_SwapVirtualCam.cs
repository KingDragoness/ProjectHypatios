using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Cinemachine;

public class Cutscene_SwapVirtualCam : CutsceneAction
{

    public GameObject targetVC;
    public CinemachineBlendDefinition.Style blendStyle;


    public override void ExecuteAction()
    {

        MainUI.Instance.cutsceneUI.cutsceneCamera.m_DefaultBlend.m_Style = blendStyle;
        if (targetVC != null)
        {
            foreach (var vc in parentCutscene.additionalVirtualCams)
            {
                vc.gameObject.SetActive(false);
            }

            targetVC.gameObject.SetActive(true);
        }

        parentCutscene.NextActionEntry();
    }

    public override void OnDone()
    {

        base.OnDone();
    }
}
