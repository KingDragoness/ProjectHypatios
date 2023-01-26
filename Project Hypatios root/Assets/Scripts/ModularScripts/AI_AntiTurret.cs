using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class AI_AntiTurret : MonoBehaviour
{

    public ModularTurretGun turretGun;
    public float rotateSpeed = 20f;
    public float force = 10f;

    private EnemyScript targetBot;
    private Vector3 currentPosTargetMove;
    private bool canSeeEnemy = false;

    private void Awake()
    {
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;


        FindEnemyTarget();

        if (targetBot != null)
        MoveAndTarget();

    }




    private void FindEnemyTarget()
    {
        targetBot = Hypatios.Enemy.FindEnemyEntity(Alliance.Player, transform.position) as EnemyScript;
    }

    private RaycastHit currentHit;
    private bool isHittingSomething = false;
    private bool isHittingTarget = false;
    
    private void RaycastSee()
    {
        RaycastHit hit;
        isHittingSomething = false;
        isHittingTarget = false;

        if (Physics.Raycast(turretGun.transform.position, turretGun.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, turretGun.layermaskWeapon, QueryTriggerInteraction.Ignore))
        {
            isHittingSomething = true;

            if (hit.collider.gameObject.IsParentOf(targetBot.transform.gameObject))
            {
                isHittingTarget = true;
            }
            else if (hit.collider.gameObject == targetBot.transform.gameObject)
            {
                isHittingTarget = true;
            }
        }

        currentHit = hit;
    }


    private void MoveAndTarget()
    {
        RaycastSee();

        var enemyScript = targetBot.GetComponent<EnemyScript>();
        Vector3 targetLook = enemyScript.OffsetedBoundWorldPosition;

        Vector3 relativePos = targetLook - transform.position;
        relativePos.z = 0;

        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);

        turretGun.transform.LookAt(targetLook);
    }


}
