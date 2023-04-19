using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHeal : MonoBehaviour
{

    public HealPlayer customCapsule;
    public float radius = .6f;
    public float explosionForce = 50f;

    // Start is called before the first frame update
    
    public void SpawnHealCapsule(int amount)
    {
        float capsuleAmount = Random.Range(Mathf.Clamp(amount-3, 2, 10), amount);
        var prefabTemplate = Hypatios.Game.Prefab_SpawnHeal;

        if (customCapsule != null)
            prefabTemplate = customCapsule;

        for (int i = 0; i < capsuleAmount; i++)
        {
            float x = Random.Range(-.5f, .5f);
            float z = Random.Range(-.5f, .5f);
            var c1 = Instantiate(prefabTemplate, transform.position + new Vector3(x, 0f, z), Quaternion.identity);
            c1.isSpawned = true;
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider c in colliders)
            {
                if (c.gameObject.layer == 15)
                {
                    Rigidbody obj = c.GetComponent<Rigidbody>();
                    if (obj != null)
                    {
                        obj.AddExplosionForce(explosionForce, transform.position, radius);
                    }
                }
            }
        }
    }
}
