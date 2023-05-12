using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracerScript : MonoBehaviour
{

    public Transform currentProjectile;
    public List<TrailRenderer> allTrails = new List<TrailRenderer>();
    private float cooldown = 5f;
    public float defaultCooldown = 5f;

    private void OnEnable()
    {
        foreach(var trail in allTrails)
        {
            trail.emitting = true;
        }

        cooldown = defaultCooldown;
    }

    public void SetTracer(Vector3 origin, Vector3 endPoint)
    {
        foreach (var trail in allTrails)
        {
            Vector3[] pos = new Vector3[2];
            pos[0] = origin;
            pos[1] = endPoint;
            trail.SetPositions(pos);
            trail.emitting = true;
        }

    }

    public void ResetTracer()
    {
        foreach (var trail in allTrails)
        {
            trail.Clear();
        }
    }

    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }

        if (currentProjectile == null)
        {
            if (cooldown > 1f)
                cooldown = 1f;
        }
        else
        {
            transform.position = currentProjectile.transform.position;
        }

        if (cooldown <= 1f)
        {
            KillTracer();
        }

        if (cooldown <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void KillTracer()
    {
        foreach (var trail in allTrails)
        {
            trail.emitting = false;
        }
    }

}
