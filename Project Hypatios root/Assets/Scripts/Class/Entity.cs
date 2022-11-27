using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Entity : MonoBehaviour
{

    [FoldoutGroup("Base")] [SerializeField] private Bounds boundingBox;

    public Bounds BoundingBox { get => boundingBox; }
    public virtual void OnDrawGizmosSelected()
    {

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.3f, 1f, 0.4f, 1f);
        Gizmos.DrawWireCube(Vector3.zero + BoundingBox.center, BoundingBox.extents);
        Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.04f);
        //Gizmos.DrawCube(Vector3.zero + boundingBox.center, boundingBox.extents);
    }

    public Vector3 OffsetedBoundPosition
    {
        get
        {
            return transform.position + Vector3.Scale(BoundingBox.center, transform.localScale);
        }
    }

    public Vector3 OffsetedBoundScale
    {
        get
        {
            return Vector3.Scale(BoundingBox.extents, transform.localScale);
        }
    }

}
