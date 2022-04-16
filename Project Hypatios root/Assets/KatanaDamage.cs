using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaDamage : MonoBehaviour
{

    GunScript katana;
    LayerMask enemyLayer = 12;
    float repulsionForce = 2;
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        katana = GetComponent<GunScript>();
        damage = katana.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (katana.isMeleeing)
        {
            if (!katana.hasHit)
            {
                if (other.gameObject.layer == enemyLayer)
                {
                    var damageReceiver = other.gameObject.GetComponentInChildren<damageReceiver>();
                    if (damageReceiver != null)
                    {
                        damageReceiver.Attacked(damage, repulsionForce);
                    }
                }
                else if (other.gameObject.layer == 10)
                {
                    Debug.Log("Hit wall!");
                }
                katana.hasHit = true;
            }
            
        }
    }
}
