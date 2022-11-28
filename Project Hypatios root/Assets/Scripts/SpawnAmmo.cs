using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAmmo : MonoBehaviour
{
    float radius = .6f;
    public int soulAmount = 1;

    // Start is called before the first frame update

    private void Start()
    {
        soulAmount += PlayerPerk.GetBonusSouls();
    }

    public void SpawnAmmoCapsule(float chance)
    {
        float randomVal = Random.Range(0f, 1f);

        if (randomVal > chance)
        {
            return;
        }


        float x = Random.Range(-.5f, .5f);
        float z = Random.Range(-.5f, .5f);
        Instantiate(Hypatios.Game.Prefab_SpawnAmmo, transform.position + new Vector3(x, 0f, z), Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.layer == 15)
            {
                Rigidbody obj = c.GetComponent<Rigidbody>();
                if (obj != null)
                {
                    obj.AddExplosionForce(50f, transform.position, radius);
                }
            }
        }
    }

    public void SpawnSoulCapsule()
    {

        float x = Random.Range(-.5f, .5f);
        float z = Random.Range(-.5f, .5f);
        var spawnSoul = Instantiate(Hypatios.Game.Prefab_SpawnSoul, transform.position + new Vector3(x, 0f, z), Quaternion.identity);
        var spawnSoulComp = spawnSoul.GetComponent<SoulCapsulePlayer>();
        spawnSoulComp.soulAmount = soulAmount;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.layer == 15)
            {
                Rigidbody obj = c.GetComponent<Rigidbody>();
                if (obj != null)
                {
                    obj.AddExplosionForce(50f, transform.position, radius);
                }
            }
        }
    }
}
