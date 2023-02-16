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
    public Alliance Alliance = Alliance.Player;
    public bool slowlyRotate = false;

    private Entity targetBot;
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
        targetBot = Hypatios.Enemy.FindEnemyEntity(Alliance, transform.position);
    }

    private RaycastHit currentHit;
    private bool isHittingSomething = false;
    private bool isHittingTarget = false;
    
    private void RaycastSee()
    {
        RaycastHit hit;
        isHittingSomething = false;
        isHittingTarget = false;

        if (Physics.Raycast(turretGun.transform.position, turretGun.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, Hypatios.Enemy.baseSolidLayer, QueryTriggerInteraction.Ignore))
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
        Vector3 targetLook = targetBot.transform.position + new Vector3(0, 0.5f, 0);

        if (enemyScript != null)
        {
            targetLook = enemyScript.OffsetedBoundWorldPosition;
        }
        else
        {

        }

        bool isBehind = false;

        {
            Vector3 turretRelative = transform.InverseTransformPoint(targetLook);

            if (turretRelative.z < 0)
                isBehind = true;
        }


        Vector3 dir = targetLook - transform.position;
        dir.z = 0;

        Quaternion toRotation = Quaternion.LookRotation(dir);

        if (!isBehind)
        {
            if (slowlyRotate == false)
            {
                turretGun.transform.LookAt(targetLook);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);
            }
            else
            {

                Vector3 dir2 = targetLook - turretGun.transform.position;

                Quaternion q2 = Quaternion.LookRotation(dir2);
                turretGun.transform.rotation = Quaternion.Lerp(turretGun.transform.rotation, q2, rotateSpeed * Time.deltaTime);
            }
        }
    }


}
