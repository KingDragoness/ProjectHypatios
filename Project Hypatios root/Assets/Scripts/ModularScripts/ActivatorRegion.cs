using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ActivatorRegion : MonoBehaviour
{
    public List<Transform> ActivatingArea;
    public List<GameObject> TargetObjectRegion = new List<GameObject>();
    public Transform player;
    public bool DEBUG_DrawGizmos = false;

    void Start()
    {
        if (player == null) player = FindObjectOfType<characterScript>().transform;
    }

    private void OnDrawGizmos()
    {

        if (DEBUG_DrawGizmos == false)
        {
            return;
        }

        foreach (Transform t in ActivatingArea)
        {
            if (t == null)
                continue;

            Gizmos.matrix = t.transform.localToWorldMatrix;
            Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.5f);
            Gizmos.DrawWireCube(Vector3.zero, t.localScale);
            Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.04f);
            Gizmos.DrawCube(Vector3.zero, t.localScale);

            {
                Vector3 v1 = t.localScale / 2f;
                Vector3 v2 = -t.localScale / 2f;
                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v1);
            }
        }

    }

    void FixedUpdate()
    {

        if (player == null)
        {
            return;
        }

        bool activate = false;

        foreach (var t in ActivatingArea)
        {
            if (activate != true)
                activate = IsInsideOcclusionBox(t, player.position);
  
        }

        foreach (GameObject go in TargetObjectRegion)
        {
            if (go.activeSelf != activate)
            {
                go.SetActive(activate);
            }
        }
    }

    [ContextMenu("Rough bound calculation")]
    public void CalculateBounds()
    {
        Bounds bounds = new Bounds();

        foreach (var go in TargetObjectRegion)
        {
            bounds.Encapsulate(GetBounds(go));
        }

        Vector3 size1 = new Vector3(bounds.size.x/2, bounds.size.y/2, bounds.size.z/2);
        size1.x = Mathf.Sqrt(size1.x);
        size1.y = Mathf.Sqrt(size1.y);
        size1.z = Mathf.Sqrt(size1.z);

        ActivatingArea[0].transform.position = bounds.center;
        ActivatingArea[0].transform.localScale = size1;

    }

    public static Bounds GetBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            //Find first enabled renderer to start encapsulate from it

            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    bounds = renderer.bounds;
                    break;
                }
            }

            //Encapsulate for all renderers

            foreach (Renderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    bounds.Encapsulate(renderer.bounds);
                }

            }
        }

        return bounds;
    }

    public void TestGizmos()
    {
        Quaternion currentRotation = this.transform.rotation;
        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(this.transform.position, Vector3.zero);

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - this.transform.position;
        bounds.center = localCenter + transform.position;
        Debug.Log("The local bounds of this model is " + bounds);

        this.transform.rotation = currentRotation;

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
