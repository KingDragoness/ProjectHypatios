using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserSmaller : MonoBehaviour
{
    public float laserWidth = .4f;
    float speedSmaller = 1f;
    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void AssignLaserWidth(float laserWidth_, float speedSmaller_ = 1)
    {
        laserWidth = laserWidth_;
        speedSmaller = speedSmaller_;
    }

    // Update is called once per frame
    void Update()
    {
        if (laserWidth > 0f)
        {
            laserWidth -= Time.deltaTime * speedSmaller;
        }
        else
        {
            Destroy(gameObject);
        }
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;
    }
}
