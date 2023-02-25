using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RandomSpawnArea : MonoBehaviour
{
    public Transform ActivatingArea;
    public bool DEBUG_DrawGizmos = false;
    public Transform test;

    void Start()
    {

    }

    [Button("Toggle RandomSpawnArea")]

    private void CloseAllRandomSpawnGizmos()
    {
        var allRandomspawns = FindObjectsOfType<RandomSpawnArea>();

        foreach(var spawn in allRandomspawns)
        {
            if (DEBUG_DrawGizmos == true)
                spawn.DEBUG_DrawGizmos = false;
            else
                spawn.DEBUG_DrawGizmos = true;
        }
    }

    private void OnDrawGizmos()
    {

        if (DEBUG_DrawGizmos == false)
        {
            return;
        }

        if (ActivatingArea == null)
            return;

        Gizmos.matrix = ActivatingArea.transform.localToWorldMatrix;
        var colorGreen = Color.green;
        colorGreen.a = 0.08f;
        Gizmos.color = colorGreen;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);//ActivatingArea.localScale/2f);
        colorGreen.a = 0.6f;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);//ActivatingArea.localScale/2f);
        Gizmos.DrawIcon(transform.position, "RandomSpawnArea.png");
    }

    void FixedUpdate()
    {

    }

    [ContextMenu("DEBUG_Test")]
    [Button("Debug_test")]
    public void DEBUG_Test()
    {
        test.transform.position = GetAnyPositionInsideBox();
    }

    [ContextMenu("DEBUG_Test")]
    [Button("testClosest")]
    public void DEBUG_TestClosest()
    {
        test.transform.position = GetClosestPoint(test.transform.position);
    }


    public Vector3 GetAnyPositionInsideBox()
    {
        float max_X = ActivatingArea.localScale.x /2f;
        float max_Y = ActivatingArea.localScale.y /2f;
        float max_Z = ActivatingArea.localScale.z /2f;

        float x = Random.Range(-max_X, max_X);
        float y = Random.Range(-max_Y, max_Y);
        float z = Random.Range(-max_Z, max_Z);

        Vector3 planeX = transform.right * x;
        Vector3 planeY = transform.up * y;
        Vector3 planeZ = transform.forward * z;

        Vector3 result = transform.position + planeX + planeY + planeZ;

        return result;
    }

    public Vector3 GetClosestPoint(Vector3 currentPos)
    {
        Vector3 relativePos = ActivatingArea.InverseTransformPoint(currentPos);
        float x_Rpos = (relativePos.x);
        float y_Rpos = (relativePos.y);
        float z_Rpos = (relativePos.z);

        float max_X = ActivatingArea.localScale.x / 2f;
        float max_Y = ActivatingArea.localScale.y / 2f;
        float max_Z = ActivatingArea.localScale.z / 2f;

        float x = Mathf.Clamp(x_Rpos * 2f * max_X, -max_X, max_X);
        float y = Mathf.Clamp(y_Rpos * 2f * max_Y, -max_Y, max_Y);
        float z = Mathf.Clamp(z_Rpos * 2f * max_Z, -max_Z, max_Z);

        Vector3 planeX = transform.right * x;
        Vector3 planeY = transform.up * y;
        Vector3 planeZ = transform.forward * z;

        Vector3 result = transform.position + planeX + planeY + planeZ;

        return result;
    }


    public bool IsInsideOcclusionBox(Vector3 aPoint)
    {
        Vector3 relativePos = ActivatingArea.InverseTransformPoint(aPoint);
        float x = Mathf.Abs(relativePos.x);
        float y = Mathf.Abs(relativePos.y);
        float z = Mathf.Abs(relativePos.z);

        if (x < (0.5f) 
            && y < (0.5f) 
            && z < (0.5f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsInsideOcclusionBox(Transform box, Vector3 aPoint)
    {
        Vector3 localPos = box.InverseTransformPoint(aPoint);

        if (Mathf.Abs(localPos.x) < (box.localScale.x / 2) && Mathf.Abs(localPos.y) < (box.localScale.y / 2) && Mathf.Abs(localPos.z) < (box.localScale.z / 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
