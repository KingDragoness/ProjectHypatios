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
    public float perSlice = 0;
    public bool disableSpawn = true;
    private Transform baitTransform;

    void Start()
    {
        float rot = 360f / number;
        baitTransform = new GameObject("baitTransform").transform;
        baitTransform.SetParent(transform.parent);
        baitTransform.position = transform.position;

        if (disableSpawn == false)
        {
            for (int x = 0; x < number; x++)
            {
                var prefab1 = Instantiate(numberAxis_t, transform);
                TextMesh label_number = prefab1.GetComponentInChildren<TextMesh>();
                prefab1.transform.localEulerAngles = new Vector3(0, 0, rot * x);
                prefab1.gameObject.SetActive(true);
                label_number.text = $"{allNumber[x]}";
            }

        }
        perSlice = rot;
    }


    /// <summary>
    /// To check roulette win.
    /// </summary>
    /// <param name="rotAxis">The ball's axis rotation.</param>
    /// <returns></returns>
    public int GetCurrentNumber(float rotAxisY)
    {
        baitTransform.transform.eulerAngles = new Vector3();
        float offsetY = transform.localEulerAngles.y;
        float nearestDist = 99999f;
        int nearestIndex = 0;

        //get nearest slice
        for (int x = 0; x < allNumber.Count; x++)
        {
            float currentAngle = x * perSlice;
            Vector3 rot = transform.eulerAngles;
            rot.y += currentAngle;

            float angle = Mathf.DeltaAngle(rot.y, rotAxisY);
            angle = Mathf.Abs(angle);

            //Debug.Log($"{currentAngle} | Delta Angle: {angle}");
            if (nearestDist > angle)
            {
                nearestDist = angle;
                nearestIndex = x;
            }

        }

        return allNumber[nearestIndex];
    }

    
}
