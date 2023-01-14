using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructibles : MonoBehaviour
{

    public int hitpoints = 100;
    public UnityEvent OnDestroy;
    public DamageToken.DamageType destroyType = DamageToken.DamageType.MiningLaser;

    public void Damage(DamageToken token)
    {
        if (destroyType == DamageToken.DamageType.Generic)
        {
            hitpoints -= Mathf.RoundToInt(token.damage);
        }
        else if (destroyType == token.damageType)
        {
            hitpoints -= Mathf.RoundToInt(token.damage);
        }

        if (hitpoints <= 0)
            Destroy();
    }

    public void Destroy()
    {
        OnDestroy?.Invoke();
        Destroy(gameObject);
    }
}
