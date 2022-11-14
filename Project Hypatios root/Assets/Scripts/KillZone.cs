using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KillZone : MonoBehaviour
{

    public List<Transform> ActivatingArea;
    public float DamagePerSecond = 10;
    public float DamageSpeedOverride = 20;
    public float DamageShakinessFactor = 0.1f;
    public bool DEBUG_DrawGizmos = false;
    [FoldoutGroup("Enemies")] public LayerMask sphereLayerMask;
    [FoldoutGroup("Enemies")] public float sphereRadius = 5f;
    [FoldoutGroup("Enemies")]
    [Tooltip("Only for explosions.")] public bool alsoDamageEnemy = false;
    [FoldoutGroup("Enemies")] public bool useEnemyKillzone = true;

    private health PlayerHealth;
    private float cooldown = 1f;
    private const float COOLDOWN_DAMAGE = 0.5f;

    void Start()
    {
        PlayerHealth = FindObjectOfType<health>();
        if (alsoDamageEnemy) DamageEnemy();
    }

    private void OnDrawGizmos()
    {

        if (DEBUG_DrawGizmos == false)
        {
            return;
        }

        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.04f);
        Gizmos.DrawSphere(transform.position, sphereRadius);

        foreach (Transform t in ActivatingArea)
        {
            if (t == null)
                continue;

            Gizmos.matrix = t.transform.localToWorldMatrix;
            Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.5f);
            Gizmos.DrawWireCube(Vector3.zero, t.localScale);
            Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.04f);
            Gizmos.DrawCube(Vector3.zero, t.localScale);

            {
                Vector3 v1 = t.localScale / 2f;
                Vector3 v2 = -t.localScale / 2f;
                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v1);
            }

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
        PlayerHealth.takeDamage(Mathf.RoundToInt(DamagePerSecond/2), DamageSpeedOverride, DamageShakinessFactor);
        SpawnIndicator.instance.Spawn(transform);

    }

    public void DamageEnemy()
    {
        Vector3 center = transform.position;

        var hitColliders = Physics.OverlapSphere(center, sphereRadius, sphereLayerMask);
        foreach (var collider in hitColliders)
        {
            var damage = collider.GetComponent<damageReceiver>();
            if (damage != null)
            {
                var token = new DamageToken(); token.origin = DamageToken.DamageOrigin.Enemy; token.damage = DamagePerSecond;

                damage.Attacked(token);
            }

        }
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
