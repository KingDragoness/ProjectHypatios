using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHeal : MonoBehaviour
{

    public GameObject healCapsule;
    public float radius = .6f;
    public float explosionForce = 50f;

    // Start is called before the first frame update
    
    public void SpawnHealCapsule(int amount)
    {
        float capsuleAmount = Random.Range(Mathf.Clamp(amount-3, 2, 10), amount);
        
        for (int i = 0; i < capsuleAmount; i++)
        {
            float x = Random.Range(-.5f, .5f);
            float z = Random.Range(-.5f, .5f);
            Instantiate(healCapsule, transform.position + new Vector3(x, 0f, z), Quaternion.identity);
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
