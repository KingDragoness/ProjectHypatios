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
    public bool isExplosion = false;
    public BaseStatusEffectObject statusEffect;
    public float statusEffectTime = 10f;
    [Range(0f,1f)] public float statusEffectChance = 0.7f;
    public bool usePrompt_StatusEffect = false;
    public string prompt_StatusEffect = "Cursed by Eclipser sword.";
    [ShowIf("isExplosion")] [FoldoutGroup("Enemies")] public Vector3 explosionDir;
    [FoldoutGroup("Enemies")] public EnemyScript originEnemy;
    [FoldoutGroup("Enemies")] public DamageToken.DamageOrigin origin = DamageToken.DamageOrigin.Enemy;
    [FoldoutGroup("Enemies")] public bool isAllowIndicator = false;
    [FoldoutGroup("Enemies")] public LayerMask sphereLayerMask;
    [FoldoutGroup("Enemies")] public float sphereRadius = 5f;
    [FoldoutGroup("Enemies")] public bool isBurn = false;
    [FoldoutGroup("Enemies")]
    [Tooltip("Only for explosions.")] public bool alsoDamageEnemy = false;
    [FoldoutGroup("Enemies")] public bool useEnemyKillzone = true;

    private PlayerHealth PlayerHealth;
    private float cooldown = 1f;
    private const float COOLDOWN_DAMAGE = 0.5f;
    private IEnumerator c1;

    //For explosion shits:
    //If Explosion() called, even if the position and setactive is on the same function/frame,
    //The OnEnabled always triggered first while the position ONLY FUCKING UPDATED in the next
    // frame which caused explosion function to trigger when the explosion prefab is at (0,0)
    //And this shit happens all of the fucking time
    void OnEnable()
    {
        PlayerHealth = Hypatios.Player.Health;
        if (alsoDamageEnemy)
        {
            if (c1 != null)
                StopCoroutine(c1);

            c1 = NextFrameDamage();
            StartCoroutine(c1);
        }
        //Debug.Log("test1");
    }

    IEnumerator NextFrameDamage()
    {
        yield return null;
        DamageEnemy();
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

            if (useEnemyKillzone) DamageEnemy();
        }
    }

    public void DamagePlayer()
    {
        PlayerHealth.takeDamage(Mathf.RoundToInt(DamagePerSecond/2), DamageSpeedOverride, DamageShakinessFactor);
        Hypatios.UI.SpawnIndicator.Spawn(transform);

        if (isExplosion)
        {
            float multiplier = explosionDir.magnitude;
            Vector3 knockbackDir = Hypatios.Player.transform.position - transform.position;
            knockbackDir.Normalize();
            knockbackDir *= multiplier;
            Hypatios.Player.Weapon.Recoil.AddCustomKnockbackForce(knockbackDir, 1f);
        }

        if (isBurn)
        {
            Hypatios.Player.Burn();
        }

        float chance = Random.Range(0f, 1f);

        if (statusEffect != null && chance < statusEffectChance)
        {
            statusEffect.AddStatusEffectPlayer(statusEffectTime);

            if (usePrompt_StatusEffect)
            {
                DeadDialogue.PromptNotifyMessage_Mod(prompt_StatusEffect, 5f);

            }
        }
    }

    /// <summary>
    /// Unreliable garbage
    /// #May 13: still unreliable garbage
    /// </summary>
    public void DamageEnemy()
    {
        Vector3 center = transform.position;

        var hitColliders = Physics.OverlapSphere(center, sphereRadius, sphereLayerMask);
        foreach (var collider in hitColliders)
        {
            var damage = collider.GetComponent<damageReceiver>();
            if (damage != null)
            {
                //Debug.Log(damage.gameObject.name);
                var token = new DamageToken(); token.origin = origin; token.damage = DamagePerSecond + (Random.Range(0, DamagePerSecond /3f)); token.originEnemy = originEnemy;
                token.isBurn = isBurn;
                token.damageType = DamageToken.DamageType.Explosion;
                if (isAllowIndicator) token.allowPlayerIndicator = isAllowIndicator;
                UniversalDamage.TryDamage(token, collider.transform, this.transform);

                //Debug.Log(token.damage);
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
