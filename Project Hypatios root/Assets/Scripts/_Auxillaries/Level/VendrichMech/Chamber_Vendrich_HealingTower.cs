using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chamber_Vendrich_HealingTower : MonoBehaviour
{

    public MechHeavenblazerEnemy Mech;
    public GameObject originHealPrefab;
    public GameObject mechHealObject;
    public GameObject sparkObject1;
    public GameObject explosionObject;

    public int Heal = 1200;
    public float HealingTime = 40f;
    public float TowerSpeedRise = 10;
    public float TowerSpeedCollapse = 25f;
    public float EnableSparkTime = 10f;
    public float normalTowerPositionY = -0.2f;
    public float introTowerPositionY = -12f;

    private float _healTimer = 40f;
    private bool hasInitiatedSpark = false;
    private bool hasHeal = false;
    private bool hasDestroyed = false;

    private void Start()
    {
        
    }

    public void Init()
    {
        _healTimer = HealingTime;
        Vector3 v = transform.position;
        v.y = introTowerPositionY;
        transform.position = v;
    }

    private void Update()
    {
        if (hasHeal == false && transform.position.y < normalTowerPositionY && hasDestroyed == false)
        {
            Vector3 v = transform.position;
            v.y += Time.deltaTime * TowerSpeedRise;
            transform.position = v;
        }

        if (hasHeal | hasDestroyed)
        {
            Vector3 v = transform.position;
            v.y -= Time.deltaTime * TowerSpeedCollapse;
            transform.position = v;
        }

        if (hasDestroyed)
            return;

        if (_healTimer > 0)
        {
            _healTimer -= Time.deltaTime;
        }
        else if (hasHeal == false)
        {
            Healing();
        }

        if (_healTimer < EnableSparkTime && !hasInitiatedSpark)
        {
            SpawnSpark();
        }
    }

    private void SpawnSpark()
    {
        hasInitiatedSpark = true;
        sparkObject1.gameObject.SetActive(true);
    }

    private void Healing()
    {
        originHealPrefab.gameObject.SetActive(true);
        mechHealObject.gameObject.SetActive(true);
        sparkObject1.gameObject.SetActive(false);
        mechHealObject.transform.position = Mech.transform.position;
        Mech.Heal(Heal);
        hasHeal = true;
        Destroy(gameObject, 6f);
    }

    public void Destroyed()
    {
        explosionObject.gameObject.SetActive(true);
        Destroy(gameObject, 6f);
        hasDestroyed = true;
    }
}
