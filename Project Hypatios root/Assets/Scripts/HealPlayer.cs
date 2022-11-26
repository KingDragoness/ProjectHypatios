using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{

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
        if (Vector3.Distance(transform.position, player.transform.position) < distanceToCollect && curHealth < playerHealth.maxHealth)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && curHealth < playerHealth.maxHealth && !playerHealth.isDead)
        {
            playerHealth.Heal(Random.Range(1, 5));
            Destroy(gameObject);
        }
    }
}
