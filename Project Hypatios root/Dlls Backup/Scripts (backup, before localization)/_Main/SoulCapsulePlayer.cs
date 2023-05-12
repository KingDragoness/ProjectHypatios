using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCapsulePlayer : MonoBehaviour
{
    GameObject player;
    public float speed;
    public float distanceToCollect; //Deprecated
    public int soulAmount = 1;
    public bool isSpawned = false;

    private float _TimeSpawned;

    // Start is called before the first frame update
    void Start()
    {
        _TimeSpawned = Time.time;
        player = Hypatios.Player.gameObject;
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
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AddSoul();
            Destroy(gameObject);
        }
    }

    public void AddSoul()
    {
        soundManagerScript.instance.Play("soul");
        Hypatios.Game.SoulPoint += soulAmount;
    }
}
