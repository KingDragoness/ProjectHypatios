using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MachineMadnessWeapon : GunScript
{

  

    public override void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        if (Hypatios.Player.disableInput)
            return;


        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Holster"))
        {
            if (Hypatios.Input.Fire1.IsPressed())
            {
                InitiateMachineUI();
            }
        }

    }

    private void InitiateMachineUI()
    {

    }

}
