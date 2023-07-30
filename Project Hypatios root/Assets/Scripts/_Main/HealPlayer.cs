﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{


    public float speed;
    public float distanceToCollect;
    public bool isSpawned = false;

    private GameObject player;
    private PlayerHealth playerHealth;
    private float curHealth;
    private float _TimeSpawned;


    // Start is called before the first frame update
    void Start()
    {
        _TimeSpawned = Time.time;
        player = Hypatios.Player.gameObject;
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        bool inDistance = false;

        if (Time.time > _TimeSpawned + 1f && isSpawned)
        {
            inDistance = true;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < distanceToCollect)
        {
            inDistance = true;
        }

        if (inDistance)
        {
            bool allowHeal = true;

            if (allowHeal)
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !playerHealth.isDead)
        {
            playerHealth.AddCacheCapsule();
            //int additionalHeal = Mathf.Clamp(Mathf.RoundToInt(playerHealth.maxHealth.Value/50f),1, 99);
            //playerHealth.Heal(Random.Range(healMin + additionalHeal, healMax + additionalHeal), instantHeal: true);
            Destroy(gameObject);
        }
    }
}
