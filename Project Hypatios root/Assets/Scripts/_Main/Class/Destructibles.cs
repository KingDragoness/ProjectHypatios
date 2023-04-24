using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destructibles : MonoBehaviour
{

    public int hitpoints = 100;
    public float minHitpointRange = -10;
    public float maxHitpointRange = 20;
    public UnityEvent OnDestroy;
    public DamageToken.DamageType destroyType = DamageToken.DamageType.MiningLaser;

    private bool hasbeenDamaged = false;

    public void Damage(DamageToken token)
    {
        bool matched = false;

        if (!hasbeenDamaged)
        {
            hitpoints += (int)Random.Range(minHitpointRange, maxHitpointRange);
            hasbeenDamaged = true;
        }

        if (destroyType == DamageToken.DamageType.Generic)
        {
            hitpoints -= Mathf.RoundToInt(token.damage);
            matched = true;
        }
        else if (destroyType == token.damageType)
        {
            hitpoints -= Mathf.RoundToInt(token.damage);
            matched = true;
        }

        if (token.origin == DamageToken.DamageOrigin.Player && matched) 
            DamageOutputterUI.instance.DisplayText(token.damage);


        if (hitpoints <= 0)
            Destroy();
    }

    public void Destroy()
    {
        OnDestroy?.Invoke();
        Destroy(gameObject);
    }
}
