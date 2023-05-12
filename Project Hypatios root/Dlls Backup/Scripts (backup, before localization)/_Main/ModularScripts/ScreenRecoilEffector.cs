using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRecoilEffector : MonoBehaviour
{

    public Vector3 rot;
    public float multiplier = 1f;

    public void RecoilEffect()
    {
        Hypatios.Player.Weapon.Recoil.CustomRecoil(rot, multiplier);
    }

}
