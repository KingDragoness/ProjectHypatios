using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NRV = non retard version
//doesnt use the retarded interval system

public class EnemyIdleSound_NRV : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    public float cooldown = 2f;

    [Range(0, 1f)]
    public float chanceToPlay = 1f;

    private bool isPlayed = false;

    private float _timer = 2f;

    private void Start()
    {
        _timer = cooldown;
    }


    void Update()
    {

        if (Time.timeScale <= 0) return;

        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }

        float chance = Random.Range(0f, 1f);

        if (chance <= chanceToPlay)
        {
            var clip = clips[Random.Range(0, clips.Length)];
            audioSource.clip = clip;
            audioSource.Play();
            isPlayed = true;
        }

        _timer = cooldown;
    }
}
