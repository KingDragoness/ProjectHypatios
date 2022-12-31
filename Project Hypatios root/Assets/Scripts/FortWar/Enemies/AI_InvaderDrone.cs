using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class AI_InvaderDrone : MonoBehaviour
{

    public Chamber_Level7 chamberScript;
    public ModularTurretGun turretGun;
    public float rotateSpeed = 20f;
    public float force = 10f;
    [FoldoutGroup("Runtime")] public RandomSpawnArea currentPatrolArea;
    [FoldoutGroup("Patrol Area")] public RandomSpawnArea patrolArea_CP1;
    [FoldoutGroup("Patrol Area")] public RandomSpawnArea patrolArea_CP2;
    [FoldoutGroup("Patrol Area")] public RandomSpawnArea patrolArea_Final;

    private EnemyScript targetBot;
    private Vector3 currentPosTargetMove;
    private bool canSeeEnemy = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (chamberScript.currentStage != Chamber_Level7.Stage.Ongoing) return; 
        
        RefreshState();

        FindEnemyTarget();

        if (targetBot != null)
            MoveAndTarget();
    }

    public void ChangeCurrentPos()
    {
        if (currentPatrolArea == null) return;
        currentPosTargetMove = currentPatrolArea.GetAnyPositionInsideBox();
    }

    private void RefreshState()
    {
        var cachePatrol = currentPatrolArea;

        if (chamberScript.GetCurrentCP().CPNumber == 1)
        {
            currentPatrolArea = patrolArea_CP1;
        }
        else if (chamberScript.GetCurrentCP().CPNumber == 2)
        {
            currentPatrolArea = patrolArea_CP2;
        }
        else if (chamberScript.GetCurrentCP().CPNumber == 0)
        {
            currentPatrolArea = patrolArea_Final;
        }

        if (cachePatrol != currentPatrolArea)
            Teleport();
    }

    private void Teleport()
    {
        rb.MovePosition(currentPatrolArea.GetAnyPositionInsideBox());
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

    private void FixedUpdate()
    {
        if (chamberScript.currentStage != Chamber_Level7.Stage.Ongoing) return;
        CheckOutBounds();

        float dist = Vector3.Distance(transform.position, currentPosTargetMove);

        if (dist > 5f)
        {
            Move();
        }
    }

    private void CheckOutBounds()
    {
        if (currentPatrolArea == null) return;
        if (currentPatrolArea.IsInsideOcclusionBox(transform.position) == false)
        {
            //Teleport();
            ChangeCurrentPos();
        }
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

    private void Move()
    {
        Vector3 direction = currentPosTargetMove - transform.position;
        rb.AddForce(direction.normalized * force * Time.deltaTime, ForceMode.VelocityChange );
    }
}
