using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFoot : MonoBehaviour
{
    Vector3 newPos, curPos, oldPos;
    public Transform body;
    public LayerMask groundLayer;
    public float legWidth;
    public float legDistance;
    public float stepDistance = .5f;
    float lerp;
    public IKFoot otherLeg;

    private void Start()
    {
        newPos = transform.position;
        curPos = transform.position;
        oldPos = transform.position;
        stepDistance = 1f;
    }

    private void Update()
    {
        transform.position = curPos;

        Ray ray = new Ray(body.position + (body.right * legWidth) + (body.forward * legDistance), Vector3.down);
        
        
        if (Physics.Raycast(ray, out RaycastHit hit, groundLayer))
        {
            Debug.DrawLine(body.position, transform.position);
            if (Vector3.Distance(newPos, hit.point) > stepDistance && otherLeg.lerp >= 1f)
            {
                lerp = 0f;
                newPos = hit.point;
            }
        }

        if (lerp < 1)
        {
            Vector3 footPos = Vector3.Lerp(oldPos, newPos, lerp);
            footPos.y += Mathf.Sin(lerp * Mathf.PI) * .4f;

            curPos = footPos;
            lerp += Time.deltaTime * 5f;
        }
        else
        {
            oldPos = newPos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPos, .2f);
    }
}

