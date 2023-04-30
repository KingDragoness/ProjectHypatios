using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLightVolume : MonoBehaviour
{

    public Color ambientColor;
    public float entrySpeed = 2f;
    public float exitSpeed = 2f;

    public void TriggerVolume()
    {
        AmbientLightController.Instance.TriggerVolume(this);
    }

    public void ExitVolume()
    {
        AmbientLightController.Instance.ExitVolume(this);
    }

}
