using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIndicator : MonoBehaviour
{
    public DamageIndicator2 damageIndicator;
    public Transform holder;
    public DamageIndicator2 spawn;

    public static SpawnIndicator instance;

    private void Awake()
    {
        instance = this;
    }

    public void Spawn(Transform target)
    {
        spawn = Instantiate(damageIndicator, holder);
        spawn.gameObject.SetActive(true);
        spawn.SetTarget(target);
    }
}
