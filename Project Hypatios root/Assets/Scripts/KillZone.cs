using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    public List<Transform> ActivatingArea;
    public float DamagePerSecond = 10;
    public float DamageSpeedOverride = 20;
    public bool DEBUG_DrawGizmos = false;

    private health PlayerHealth;
    private float cooldown = 1f;
    private const float COOLDOWN_DAMAGE = 0.5f;

    void Start()
    {
        PlayerHealth = FindObjectOfType<health>();
    }

    private void OnDrawGizmos()
    {

        if (DEBUG_DrawGizmos == false)
        {
            return;
        }

        foreach (Transform t in ActivatingArea)
        {
            if (t == null)
                continue;

            Gizmos.matrix = t.transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, t.localScale);
        }

    }

    void FixedUpdate()
    {

        if (PlayerHealth == null)
        {
            return;
        }

        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }
        else
        {
            cooldown = COOLDOWN_DAMAGE;
        }

        bool activate = false;

        foreach (var t in ActivatingArea)
        {
            if (activate != true)
                activate = IsInsideOcclusionBox(t, PlayerHealth.transform.position);

            if (activate)
            {
                DamagePlayer();
            }
        }
    }

    public void DamagePlayer()
    {
        PlayerHealth.takeDamage(Mathf.RoundToInt(DamagePerSecond/2), DamageSpeedOverride);
        SpawnIndicator.instance.Spawn(transform);
    }

    public static bool IsInsideOcclusionBox(Transform box, Vector3 aPoint)
    {
        Vector3 localPos = box.InverseTransformPoint(aPoint);

        if (Mathf.Abs(localPos.x) < (box.localScale.x / 2) && Mathf.Abs(localPos.y) < (box.localScale.y / 2) && Mathf.Abs(localPos.z) < (box.localScale.z / 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
