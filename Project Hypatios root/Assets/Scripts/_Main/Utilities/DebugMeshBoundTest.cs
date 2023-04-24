using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMeshBoundTest : MonoBehaviour
{

    public Bounds FinalBound;

    private List<Bounds> allBounds = new List<Bounds>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.35f, 0.7f, 0.5f, 0.1f);
        foreach (var bound in allBounds)
        {
            Gizmos.DrawWireCube(bound.center, bound.size);
        }

        Gizmos.color = new Color(0.4f, 0.6f, 0.5f, 0.6f);
        Gizmos.DrawWireCube(FinalBound.center, FinalBound.size);

    }

    [ContextMenu("TestGizmos")]
    public void TestGizmos()
    {
        allBounds.Clear();
        var allParents = gameObject.GetAllParents();
        List<Quaternion> currentParentQuarternions = new List<Quaternion>();

        foreach (var parentT in allParents)
        {
            currentParentQuarternions.Add(parentT.transform.rotation);
            //parentT.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        Quaternion currentRotation = this.transform.rotation;
        //this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Bounds bounds = new Bounds(this.transform.position, Vector3.zero);

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            bounds.Encapsulate(renderer.bounds);
            allBounds.Add(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - this.transform.position;
        bounds.center = localCenter + transform.position;
        FinalBound = bounds;
        Debug.Log("The local bounds of this model is " + bounds);

        //this.transform.rotation = currentRotation;

        for (int x = 0; x < currentParentQuarternions.Count; x++)
        {
            var quart = currentParentQuarternions[x];
            //allParents[x].transform.rotation = quart;
        }

    }

    private void CalculateLocalBounds()
    {

    }
}
