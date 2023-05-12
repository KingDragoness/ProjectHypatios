using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSystemManager : MonoBehaviour
{
    public float particleSystemTime = .5f;
    private void Update()
    {
        particleSystemTime -= Time.deltaTime;
        if (particleSystemTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
