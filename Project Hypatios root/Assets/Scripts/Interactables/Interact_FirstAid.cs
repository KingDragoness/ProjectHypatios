using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_FirstAid : MonoBehaviour
{

    public float healAmount = 10;
    public GameObject available;
    public GameObject unavailable;
    public AudioSource audio_FirstAid;

    private bool hasHealed = false;
    private float timeLastHealed = 1f;
    private float cooldownRefresh = 60f;

    private void Update()
    {
        if (hasHealed == false) return;

        if (timeLastHealed + cooldownRefresh < Time.time)
        {
            hasHealed = false;
            RefreshIcon();
        }
    }

    public void HealthUse()
    {
        if (hasHealed)
        {
            RefreshIcon();
            return;
        }

        CharacterScript characterScript = FindObjectOfType<CharacterScript>();
        characterScript.Health.targetHealth = characterScript.Health.curHealth + Mathf.RoundToInt(healAmount);
        characterScript.Health.HealthSpeed = 30f;
        audio_FirstAid.Play();

        hasHealed = true;
        timeLastHealed = Time.time;
        RefreshIcon();
    }

    private void RefreshIcon()
    {
        if (hasHealed)
        {
            available.gameObject.SetActive(false);
            unavailable.gameObject.SetActive(true);
        }
        else
        {
            available.gameObject.SetActive(true);
            unavailable.gameObject.SetActive(false);
        }
    }
}
