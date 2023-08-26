using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillAmmoPlayer : MonoBehaviour
{
    GameObject player;
    public float speed;
    public float distanceToCollect;
    public bool isSpawned = false;

    float curHealth;

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
            if (Hypatios.Player.Weapon.playerMode == Player.Aldrich)
            {
                var gun = Hypatios.Player.Weapon.GetRandomGun();
                var weaponData = Hypatios.Assets.GetWeapon(gun.weaponName);

                float randomTime = Random.Range(0f, 1f);
                int ammoAmount = Mathf.RoundToInt(weaponData.rewardRate.Evaluate(randomTime));

                Hypatios.Player.Weapon.RefillAmmo(gun, ammoAmount);
            } 

            Destroy(gameObject);
        }
    }
}
