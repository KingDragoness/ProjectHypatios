using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectileEnemy : MonoBehaviour
{

    public Transform origin;
    public float force;
    public float variableForce;
    public HitAndDamageProjectile projectile;

    public void SpawnProjectile()
    {
        var prefab1 = FireProjectile();
        prefab1.gameObject.SetActive(true);

    }

    public HitAndDamageProjectile FireProjectile()
    {
        var projectile1 = Instantiate(projectile, origin.transform.position, origin.transform.rotation);
        projectile1.rigidbody.AddForce(origin.transform.forward * (force + Random.Range(0f, variableForce)));

        return projectile1;
    }

    public HitAndDamageProjectile FireProjectile(Vector3 dir)
    {
        var projectile1 = Instantiate(projectile, origin.transform.position, origin.transform.rotation);
        projectile1.rigidbody.AddForce(dir * (force + Random.Range(0f, variableForce)));

        return projectile1;
    }
}
