using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Elena1_PathfinderSecurity : MonoBehaviour
{

    public Transform target;
    public float distanceLimit = 5;
    private NavMeshAgent NavMeshAgent;

    private void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, target.position) < distanceLimit)
        {
            Destroy(gameObject);
        }
        else
        {
            NavMeshAgent.SetDestination(target.position);
        }
    }

    public void SpawnCopy()
    {
        GameObject copyObject = Instantiate(gameObject);
        copyObject.transform.position = gameObject.transform.position;
        copyObject.SetActive(true);
    }

}
