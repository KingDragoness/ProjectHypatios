using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyTest : MonoBehaviour
{
    public Transform Player;
    public int MoveSpeed = 4;
    public int MaxDist = 10;
    public int MinDist = 5;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = Player.transform.position + new Vector3(0, 3, 0);

        transform.LookAt(offset);

        if (Vector3.Distance(transform.position, Player.position) >= MinDist)
        {

            if (rb == null)
            {
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
            }
            else
            {
                rb.AddForce(transform.forward * MoveSpeed * 100 * Time.deltaTime) ;
            }

            if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
            {
                //Here Call any function U want Like Shoot at here or something
            }

        }
    }
}
