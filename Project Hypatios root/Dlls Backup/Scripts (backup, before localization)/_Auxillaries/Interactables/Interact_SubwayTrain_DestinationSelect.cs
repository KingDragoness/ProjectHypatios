using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevLocker.Utils;

public class Interact_SubwayTrain_DestinationSelect : MonoBehaviour
{

    public Interact_SubwayTrain subwayScript;
    public SceneReference sceneTarget;
    public GameObject activatedMode;
    public GameObject unactivatedMode;
    public Interact_Touchable touchScript;

    public void SelectDestination()
    {
        subwayScript.currentDestination = this;
    }

    public void Activate()
    {
        activatedMode.gameObject.SetActive(true);
        unactivatedMode.gameObject.SetActive(false);
    }

    public void Unactive()
    {
        activatedMode.gameObject.SetActive(false);
        unactivatedMode.gameObject.SetActive(true);
    }

}
