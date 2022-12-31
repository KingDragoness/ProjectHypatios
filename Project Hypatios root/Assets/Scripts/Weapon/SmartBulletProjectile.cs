using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SmartBulletProjectile : MonoBehaviour
{
    public float speedRotate = 10;
    public bool isDisableAutoTrack = false;
    public bool isSuperSmartBullet = false;
    [ShowIf("isSuperSmartBullet")]
    public LayerMask layerMask;
    public Vector3 forceAdd;

    public EnemyScript enemyTarget;
    public Vector3 cachedTargetPos;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public RaycastHit GetHit()
    {
        RaycastHit hit;
        var dir = enemyTarget.OffsetedBoundWorldPosition - transform.position;
        dir.Normalize();

        if (Physics.Raycast(transform.position, dir, out hit, 1000f, layerMask, QueryTriggerInteraction.Ignore))
        {

        }
        else
        {
            hit.point = dir * 1000f;
        }

        return hit;

    }

    private bool bulletCanSee = false;

    private void FixedUpdate()
    {
        float a = 1;
        bulletCanSee = false;

        if (enemyTarget != null)
        {
            var offsetPos = enemyTarget.OffsetedBoundWorldPosition;
            var q = Quaternion.LookRotation(offsetPos - transform.position);
            RaycastHit hit = new RaycastHit();

            if (isSuperSmartBullet)
            {
                hit = GetHit();

                if (hit.collider != null)
                {
                    //Debug.Log(hit.collider.name);
                    var damageReceiver = hit.collider.gameObject.GetComponentThenChild<damageReceiver>();
                    var enemyScript = hit.collider.gameObject.GetComponentInParent<EnemyScript>();

                    if (damageReceiver != null | enemyScript != null)
                    {
                        bulletCanSee = true;

                    }    
                }

                if (bulletCanSee == false)
                {
                    q = Quaternion.LookRotation(cachedTargetPos - transform.position);
                }
            }
            else
            {
            }



            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);

            Vector3 relativePos = transform.InverseTransformPoint(enemyTarget.transform.position);
            relativePos.z = 0f;
            float distXYPlane = Vector3.Distance(transform.position, relativePos);
            float dist = Vector3.Distance(transform.position, enemyTarget.transform.position);

            if (distXYPlane > 3f)
            {
                a = Mathf.Clamp(1f - (distXYPlane - 3f) * 0.04f, 0f ,1f);
            }
            else
            {
                a = 1f;
            }

            a += Mathf.Clamp(dist * 0.025f, 0, 0.55f);

            if (isSuperSmartBullet)
            {
                if (bulletCanSee)
                {

                }
                else
                {
                    a = 0.6f;
                }
            }
        }
        else if (isDisableAutoTrack == false)
        {
            var q = Quaternion.LookRotation(cachedTargetPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);

        }

        float _speedMod = Mathf.Clamp(a, 0.3f, 1f);

        rb.AddRelativeForce(forceAdd * _speedMod * Time.deltaTime);
    }

    public void SetLookImmediately()
    {
        if (enemyTarget != null)
        {
            var q = Quaternion.LookRotation(enemyTarget.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, speedRotate * Time.deltaTime);
        }
    }

}
