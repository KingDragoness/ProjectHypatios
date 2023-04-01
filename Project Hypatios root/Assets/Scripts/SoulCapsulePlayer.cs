using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCapsulePlayer : MonoBehaviour
{
    GameObject player;
    public float speed;
    public float distanceToCollect;
    public int soulAmount = 1;


    // Start is called before the first frame update
    void Start()
    {
        player = Hypatios.Player.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector3.Distance(transform.position, player.transform.position) < distanceToCollect)
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
