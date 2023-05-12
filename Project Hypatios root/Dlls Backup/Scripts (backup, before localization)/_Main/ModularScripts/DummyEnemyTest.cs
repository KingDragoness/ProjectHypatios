using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyTest : MonoBehaviour
{
    public int MoveSpeed = 4;
    public int MaxDist = 10;
    public int MinDist = 5;
    public bool disableBehavior = false;
    public float rotationSpeed = 7.5f;

    private Rigidbody rb;

    public Rigidbody Rigidbody { get => rb; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (disableBehavior) return;

        Vector3 targetLook = Hypatios.Player.transform.position + new Vector3(0, 3, 0);

        Vector3 relativePos = targetLook - transform.position;

        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, Hypatios.Player.transform.position) >= MinDist)
        {

            if (rb == null)
            {
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
            }
            else
            {
                rb.AddRelativeForce(Vector3.forward * MoveSpeed * 100 * Time.deltaTime) ;
            }

            if (Vector3.Distance(transform.position, Hypatios.Player.transform.position) <= MaxDist)
            {

            }

        }
    }
}
