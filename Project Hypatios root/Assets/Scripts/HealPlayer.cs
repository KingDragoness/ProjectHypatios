using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{

    public int healMin = 1;
    public int healMax = 5;
    GameObject player;
    public float speed;
    public float distanceToCollect;
    PlayerHealth playerHealth;
    float curHealth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        curHealth = playerHealth.targetHealth;
        if (Vector3.Distance(transform.position, player.transform.position) < distanceToCollect && curHealth < playerHealth.maxHealth.Value)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && curHealth < playerHealth.maxHealth.Value && !playerHealth.isDead)
        {
            int additionalHeal = Mathf.Clamp(Mathf.RoundToInt(playerHealth.maxHealth.Value/50f),1, 99);
            playerHealth.Heal(Random.Range(healMin + additionalHeal, healMax + additionalHeal), instantHeal: true);
            Destroy(gameObject);
        }
    }
}
