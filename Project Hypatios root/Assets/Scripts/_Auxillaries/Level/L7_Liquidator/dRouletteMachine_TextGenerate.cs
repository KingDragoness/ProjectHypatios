using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dRouletteMachine_TextGenerate : MonoBehaviour
{

    public int number = 37;
    public Transform numberAxis_t;
    public List<int> allNumber = new List<int>()
    {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26
    };

    void Start()
    {
        float rot = 360f / number;

        for(int x = 0; x < number; x++)
        {
            var prefab1 = Instantiate(numberAxis_t, transform);
            TextMesh label_number = prefab1.GetComponentInChildren<TextMesh>();
            prefab1.transform.localEulerAngles = new Vector3(0, 0, rot * x);
            prefab1.gameObject.SetActive(true);
            label_number.text = $"{allNumber[x]}";
        }
    }

    
}
