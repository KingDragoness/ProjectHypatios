using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_FirstAid : MonoBehaviour
{

    public float healAmount = 10;
    public AudioSource audio_FirstAid;

    private bool hasHealed = false;

    public void HealthUse()
    {
        //if (hasHealed) return;

        characterScript characterScript = FindObjectOfType<characterScript>();
        characterScript.heal.targetHealth = characterScript.heal.curHealth + Mathf.RoundToInt(healAmount);
        audio_FirstAid.Play();

        hasHealed = true;

    }

}
