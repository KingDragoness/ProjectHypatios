using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_VerticalBooster : MonoBehaviour
{

    public Vector3 forceGlobalDir = new Vector3(0,1,0);
    public float multiplier = 1f;

    public void BoostPlayer()
    {
        Hypatios.Player.rb.AddForce(forceGlobalDir * multiplier, ForceMode.VelocityChange);
    }

}
