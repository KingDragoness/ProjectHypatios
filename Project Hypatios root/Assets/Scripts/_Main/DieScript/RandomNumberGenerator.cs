using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumberGenerator : MonoBehaviour
{

    public TextMesh textMesh;

    private void FixedUpdate()
    {
        textMesh.text = Random.Range(1000000, 99999999).ToString() + Random.Range(1000000, 99999999).ToString();
    }

    public static string RandomText(int pow)
    {
        float lowNumber = Mathf.Pow(10, pow);
        float highNumber = Mathf.Pow(10, pow+1);
        return Random.Range(lowNumber, highNumber).ToString() + Random.Range(lowNumber, highNumber).ToString();
    }
}
