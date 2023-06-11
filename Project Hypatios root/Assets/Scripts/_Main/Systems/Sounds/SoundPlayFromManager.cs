using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayFromManager : MonoBehaviour
{

    public List<string> audioNames = new List<string>();

    public void PlaySound()
    {
        soundManagerScript.instance.Play3D(audioNames[Random.Range(0,audioNames.Count-1)], transform.position);

    }

}
