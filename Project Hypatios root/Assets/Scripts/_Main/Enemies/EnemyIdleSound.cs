using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleSound : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip[] clips;
    public int interval = 1;

    [Range(0,1f)]
    public float chanceToPlay = 1f;

    private bool isPlayed = false;

    private void Start()
    {
        if (interval <= 1)
        {
            interval = 2;
        }
    }

    void Update()
    {

        if (Time.timeScale > 0 && Mathf.RoundToInt(Time.time * 10) % interval == 1) { isPlayed = false; }

        if (Mathf.RoundToInt(Time.time * 10) % interval == 0 && isPlayed == false)
        {
            float chance = Random.Range(0f, 1f);

            if (chance <= chanceToPlay)
            {
                var clip = clips[Random.Range(0, clips.Length)];
                audioSource.clip = clip;
                audioSource.Play();
                isPlayed = true;
            }
        }
    }
}
