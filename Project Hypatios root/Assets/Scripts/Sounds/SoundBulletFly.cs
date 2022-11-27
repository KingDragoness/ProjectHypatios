using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBulletFly : MonoBehaviour
{


    private void OnEnable()
    {
        float chance1 = Random.Range(0f, 1f);

        if (chance1 > 0.5f)
            soundManagerScript.instance.Play3D("flybullet.0", transform.position);
        else
            soundManagerScript.instance.Play3D("flybullet.1", transform.position);
    }

}
