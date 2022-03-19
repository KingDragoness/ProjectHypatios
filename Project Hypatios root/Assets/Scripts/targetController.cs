using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetController : MonoBehaviour
{
    
    [SerializeField] GameObject target1;
    [SerializeField] GameObject target2;
    Vector3 target1Pos;
    Vector3 target2Pos;
    Vector3 curTarget;
    public float moveSpeed = 1f;
    public float distanceToChangeDir = .5f;
    float distance;

    bool targetis1 = true;
    // Start is called before the first frame update
    void Start()
    {
        target1Pos = target1.transform.position;
        target2Pos = target2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (targetis1)
        {
            curTarget = target1Pos;
        }
        else
        {
            curTarget = target2Pos;
        }
        distance = Vector3.Distance(transform.position, curTarget);
        if (distance > distanceToChangeDir)
        {
            transform.position += (curTarget - transform.position).normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            targetis1 = !targetis1;
        }
    }
}
