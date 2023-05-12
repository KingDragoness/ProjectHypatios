using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ModularMissileTurretLauncher : MonoBehaviour
{

    public MissileChameleon missilePrefab;
    public Transform[] allOuts;
    public float perFireInterval = 0.2f;
    public Alliance alliance;
    [FoldoutGroup("Audios")] public AudioSource audio_Fire;

    private Entity currentTarget;
    private bool hasShot = false;

    private void OnEnable()
    {
        currentTarget = Hypatios.Enemy.FindEnemyEntity(alliance, transform.position);
    }

    [Button("Fire amount")]
    public void Debug_Fire(int amount = 4)
    {
        StartCoroutine(MissileFiring(amount));
    }

    public IEnumerator MissileFiring(int amount)
    {
        for (int x = 0; x < amount; x++)
        {
            Transform targetOut = allOuts[0];
            if (allOuts.Length > x)
                targetOut = allOuts[x];
            else
            {
                int index1 = x % allOuts.Length;
                targetOut = allOuts[index1];
            }

            FireMissile(targetOut);
            yield return new WaitForSeconds(perFireInterval);
        }
    }


    [Button("Fire missile")]
    public void FireMissile(Transform outOrigin)
    {
        GameObject prefab1 = Instantiate(missilePrefab.gameObject, outOrigin.position, outOrigin.rotation);
        prefab1.gameObject.SetActive(true);
        var missile = prefab1.GetComponent<MissileChameleon>();
        missile.OverrideTarget(currentTarget, alliance);
        audio_Fire.Play();
        hasShot = true;
    }
}
