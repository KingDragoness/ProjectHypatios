using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySwordProjectile : MonoBehaviour
{

    public float distanceHit = 20f;
    public float moveSpeed = 100f;
    public float rotateSpeed = 10f;
    public float angleLimit = 3f;
    public GameObject holyExplosion;
    public LayerMask layerMask;

    private bool hasStuckOnTheGround = false;
    private bool hasLaunched = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * distanceHit);
    }

    private void Update()
    {
        if (hasStuckOnTheGround == true)
            return;

        var target = Hypatios.Player.transform.position;
        Vector3 dir = target - transform.position;

        if (hasLaunched)
        {
            LaunchAndMove();
        }

        if (!hasLaunched)
        {
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }

        float angle = Vector3.Angle(dir, transform.forward);
        Vector3 relative = transform.InverseTransformPoint(target);

        if (angle < angleLimit && relative.z > 0)
            hasLaunched = true;

        if (transform.position.y < -1000)
        {
            Destroy(gameObject);
        }

    }

    private void LaunchAndMove()
    {
        var target = Hypatios.Player.transform.position;

        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
    }

    private void FixedUpdate()
    {
        if (hasStuckOnTheGround == true)
            return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceHit, layerMask, QueryTriggerInteraction.Ignore))
        {
            //hit something and spawn holy sword explosion
            StuckToGround();

            var newExplosion = Instantiate(holyExplosion, hit.point, holyExplosion.transform.rotation);
            newExplosion.gameObject.SetActive(true);
            Destroy(newExplosion.gameObject, 7f);
        }
    }

    private void StuckToGround()
    {
        hasStuckOnTheGround = true;
    }

}
