using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillAmmoPlayer : MonoBehaviour
{
    GameObject player;
    public float speed;
    public float distanceToCollect;
    WeaponManager weaponManager;
    float curHealth;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        weaponManager = player.GetComponentInChildren<WeaponManager>();
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
            var gun = weaponManager.GetRandomGun();
            var weaponData = weaponManager.GetWeaponItemData(gun);

            float randomTime = Random.Range(0f, 1f);
            int ammoAmount = Mathf.RoundToInt(weaponData.rewardRate.Evaluate(randomTime));

            weaponManager.RefillAmmo(gun, ammoAmount);
            Destroy(gameObject);
        }
    }
}
