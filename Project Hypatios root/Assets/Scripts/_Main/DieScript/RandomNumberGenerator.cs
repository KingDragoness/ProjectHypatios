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
}
