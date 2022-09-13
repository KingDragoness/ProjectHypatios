using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnArea : MonoBehaviour
{
    public Transform ActivatingArea;
    public bool DEBUG_DrawGizmos = false;
    public Transform test;

    void Start()
    {

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
        Gizmos.DrawCube(Vector3.zero, ActivatingArea.localScale);
        colorGreen.a = 0.6f;
        Gizmos.DrawWireCube(Vector3.zero, ActivatingArea.localScale);
        Gizmos.DrawIcon(transform.position, "RandomSpawnArea.png");
    }

    void FixedUpdate()
    {

    }

    [ContextMenu("DEBUG_Test")]
    public void DEBUG_Test()
    {
        test.transform.position = GetAnyPositionInsideBox();
    }

    public Vector3 GetAnyPositionInsideBox()
    {
        float max_X = ActivatingArea.localScale.x;
        float max_Y = ActivatingArea.localScale.y;
        float max_Z = ActivatingArea.localScale.z;

        float x = Random.Range(-max_X, max_X);
        float y = Random.Range(-max_Y, max_Y);
        float z = Random.Range(-max_Z, max_Z);

        Vector3 planeX = transform.right * x;
        Vector3 planeY = transform.up * y;
        Vector3 planeZ = transform.forward * z;

        Vector3 result = transform.position + planeX + planeY + planeZ;

        return result;
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
