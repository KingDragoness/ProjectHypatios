using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class AI_TurretMachineGun : MonoBehaviour
{

    public ModularTurretGun turretGun;
    public Transform primaryPivot;
    public float rotateSpeed = 20f;
    public Alliance Alliance = Alliance.Player;
    public float limitAxisX = 4f;
    public float minAxisY = 4f;

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



    private void MoveAndTarget()
    {
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
            Transform mainPivot = transform;
            if (primaryPivot != null)
                mainPivot = primaryPivot;

            Vector3 turretRelative = mainPivot.InverseTransformPoint(targetLook);

            if (turretRelative.z < 0)
                isBehind = true;
        }

        Vector3 dir = targetLook - transform.position;
        dir.z = 0;

        if (!isBehind)
        {
            Vector3 dir2 = targetLook - transform.position;
            dir2.y = Mathf.Clamp(dir2.y, -limitAxisX, limitAxisX);

            Quaternion q2 = Quaternion.LookRotation(dir2);
            transform.rotation = Quaternion.Lerp(transform.rotation, q2, rotateSpeed * Time.deltaTime);
        }
    }

}
