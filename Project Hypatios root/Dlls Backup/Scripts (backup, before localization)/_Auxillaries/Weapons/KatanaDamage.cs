using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaDamage : MonoBehaviour
{

    public KatanaScript katana;
    LayerMask enemyLayer = 12;
    float repulsionForce = 2;
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        damage = katana.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer)
        {
            var damageReceiver = other.gameObject.GetComponentInChildren<damageReceiver>();
            if (damageReceiver != null)
            {
                katana.DamageEnemy(damageReceiver);
            }
        }
        else if (other.gameObject.layer == 10)
        {

        }
    }
}
