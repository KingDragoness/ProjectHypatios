using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidDamageHitbox : MonoBehaviour
{

    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            health h = other.gameObject.GetComponent<health>();

            if (h != null)
            {
                h.takeDamage(damage);
                var spawn = FindObjectOfType<SpawnIndicator>();
                spawn.Spawn(transform);
            }
        }
    }
}
